using System.Text.Json.Nodes;

namespace RedGraniteCms.Server.Core.Models;

// =============================================================================
// Item
// =============================================================================

/// <summary>
/// Universal CMS content node — analogous to Drupal's node or WordPress's post.
/// All CMS artifacts (user profiles, business profiles, blog posts, social media
/// feeds, wiki entries, CMS pages, etc.) are Items, differentiated by
/// <see cref="ContentType"/>. Type-specific fields live in <see cref="Metadata"/>.
///
/// Inherits lifecycle, visibility, slug, and language from <see cref="ContentEntity"/>.
/// Inherits identity and audit from <see cref="AuditableEntity"/>.
/// </summary>
public class Item : ContentEntity
{
    // -------------------------------------------------------------------------
    // Core identity
    // -------------------------------------------------------------------------

    /// <summary>
    /// The user or entity that owns this content.
    /// Distinct from CreatedBy — an admin may create content on behalf of an owner.
    /// </summary>
    public string OwnerId { get; private set; } = string.Empty;

    /// <summary>
    /// Discriminator for the content type — e.g. "user-profile", "business-profile",
    /// "blog-post", "wiki-entry", "cms-page", "feed-post".
    /// Consumers use this to interpret <see cref="Metadata"/> correctly.
    /// </summary>
    public string ContentType { get; private set; } = string.Empty;

    // -------------------------------------------------------------------------
    // Display
    // -------------------------------------------------------------------------

    /// <summary>The display name or headline of the content.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Short summary shown in listings, search results, and previews.
    /// Null for content types with no meaningful preview (e.g. feed posts).
    /// </summary>
    public string? Summary { get; private set; }

    /// <summary>
    /// The main content body. Typically HTML or Markdown.
    /// Null for drafts or content types with no body (e.g. redirect pages).
    /// </summary>
    public string? Content { get; private set; }

    // -------------------------------------------------------------------------
    // Hierarchy & ordering
    // -------------------------------------------------------------------------

    /// <summary>Direct parent Item ID, if any (e.g. wiki section → page).</summary>
    public string? ParentId { get; private set; }

    /// <summary>
    /// Ordered ancestor IDs from root to immediate parent.
    /// Enables efficient tree queries without recursive joins.
    /// </summary>
    public IReadOnlyList<string> AncestorIds { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Explicit sort position among siblings under the same parent.
    /// Lower values appear first. Defaults to 0.
    /// </summary>
    public int SortOrder { get; private set; }

    // -------------------------------------------------------------------------
    // Extension points
    // -------------------------------------------------------------------------

    /// <summary>
    /// Structured JSON store for type-specific fields and media references.
    /// EF Core 8+ maps this directly to a JSON column.
    ///
    /// Example shape for a blog post:
    /// {
    ///   "seo": { "title": "...", "description": "...", "canonical": "..." },
    ///   "og":  { "image": "https://...", "type": "article" },
    ///   "hero_image_url": "https://..."
    /// }
    /// </summary>
    public JsonObject Metadata { get; private set; } = new JsonObject();

    /// <summary>Case-insensitive folksonomy tags.</summary>
    public IReadOnlySet<string> Tags { get; private set; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    // -------------------------------------------------------------------------
    // Constructors
    // -------------------------------------------------------------------------

    protected Item() { } // For deserialization / ORM

    private Item(
        string ownerId,
        string contentType,
        string title,
        string? summary,
        string? content,
        ItemStatus status,
        ItemVisibility visibility,
        string language,
        string? slug,
        string? parentId,
        IEnumerable<string>? ancestorIds,
        int sortOrder,
        JsonObject? metadata,
        IEnumerable<string>? tags,
        string? createdBy
    ) : base(createdBy)
    {
        OwnerId = Guard(ownerId, nameof(ownerId));
        ContentType = Guard(contentType, nameof(contentType));
        Title = Guard(title, nameof(title));
        Summary = summary;
        Content = content;
        Status = status;
        Visibility = visibility;
        Language = Guard(language, nameof(language));
        Slug = slug is not null ? GuardSlug(slug, nameof(slug)) : null;
        ParentId = parentId;
        AncestorIds = ancestorIds?.ToList().AsReadOnly() ?? Array.Empty<string>().ToList().AsReadOnly();
        SortOrder = sortOrder;
        Metadata = metadata is not null ? JsonNode.Parse(metadata.ToJsonString())!.AsObject() : new JsonObject();
        Tags = tags is not null
            ? new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // Factory
    // -------------------------------------------------------------------------

    public static Item Create(
        string ownerId,
        string contentType,
        string title,
        string? summary = null,
        string? content = null,
        ItemStatus status = ItemStatus.Draft,
        ItemVisibility visibility = ItemVisibility.Private,
        string language = "en",
        string? slug = null,
        string? parentId = null,
        IEnumerable<string>? ancestorIds = null,
        int sortOrder = 0,
        JsonObject? metadata = null,
        IEnumerable<string>? tags = null,
        string? createdBy = null
    ) => new(ownerId, contentType, title, summary, content,
             status, visibility, language, slug,
             parentId, ancestorIds, sortOrder,
             metadata, tags, createdBy);

    // -------------------------------------------------------------------------
    // Mutation
    // -------------------------------------------------------------------------

    public void Update(
        string title,
        string? summary = null,
        string? content = null,
        ItemStatus? status = null,
        ItemVisibility? visibility = null,
        string? language = null,
        string? slug = null,
        string? contentType = null,
        string? parentId = null,
        IEnumerable<string>? ancestorIds = null,
        int? sortOrder = null,
        JsonObject? metadata = null,
        IEnumerable<string>? tags = null,
        string? modifiedBy = null
    )
    {
        Title = Guard(title, nameof(title));
        Summary = summary;
        Content = content;

        if (status is not null) Status = status.Value;
        if (visibility is not null) Visibility = visibility.Value;
        if (language is not null) Language = Guard(language, nameof(language));
        if (slug is not null) Slug = GuardSlug(slug, nameof(slug));
        if (contentType is not null) ContentType = Guard(contentType, nameof(contentType));
        if (parentId is not null) ParentId = parentId;
        if (ancestorIds is not null) AncestorIds = ancestorIds.ToList().AsReadOnly();
        if (sortOrder is not null) SortOrder = sortOrder.Value;
        if (metadata is not null) Metadata = JsonNode.Parse(metadata.ToJsonString())!.AsObject();
        if (tags is not null) Tags = new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase);

        UpdateLastModified(modifiedBy);
    }

    // -------------------------------------------------------------------------
    // Tag helpers
    // -------------------------------------------------------------------------

    /// <summary>Adds a tag if not already present (case-insensitive).</summary>
    public void AddTag(string tag, string? modifiedBy = null)
    {
        var set = new HashSet<string>(Tags, StringComparer.OrdinalIgnoreCase) { tag };
        Tags = set;
        UpdateLastModified(modifiedBy);
    }

    /// <summary>Removes a tag if present (case-insensitive).</summary>
    public void RemoveTag(string tag, string? modifiedBy = null)
    {
        var set = new HashSet<string>(Tags, StringComparer.OrdinalIgnoreCase);
        if (set.Remove(tag))
        {
            Tags = set;
            UpdateLastModified(modifiedBy);
        }
    }

    // -------------------------------------------------------------------------
    // Metadata helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Gets a top-level metadata value cast to the requested JsonNode type.
    /// Example: GetMetadata&lt;JsonObject&gt;("seo")?["title"]?.GetValue&lt;string&gt;()
    /// </summary>
    public T? GetMetadata<T>(string key) where T : JsonNode
        => Metadata[key] as T;

    /// <summary>Gets a top-level scalar string value by key.</summary>
    public string? GetMetadataString(string key)
        => Metadata[key]?.GetValue<string>();

    /// <summary>
    /// Sets or removes a top-level metadata key.
    /// Value can be any JsonNode — a string, number, object, or array.
    /// Pass null to remove the key.
    /// </summary>
    public void SetMetadata(string key, JsonNode? value, string? modifiedBy = null)
    {
        if (value is null)
            Metadata.Remove(key);
        else
            Metadata[key] = JsonNode.Parse(value.ToJsonString());

        UpdateLastModified(modifiedBy);
    }

    // -------------------------------------------------------------------------
    // Guards
    // -------------------------------------------------------------------------

    private static string Guard(string value, string paramName)
        => string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value cannot be null or empty.", paramName)
            : value;

    private static string GuardSlug(string value, string paramName)
    {
        Guard(value, paramName);

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-z0-9]+(?:-[a-z0-9]+)*$"))
            throw new ArgumentException(
                "Slug must be lowercase alphanumeric words separated by hyphens.", paramName);

        return value;
    }
}
