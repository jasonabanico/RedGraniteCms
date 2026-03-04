namespace RedGraniteCms.Server.Core.Models;

// =============================================================================
// ContentEntity
// =============================================================================

/// <summary>
/// Base class for all CMS content entities — anything that has a publish
/// lifecycle, an access visibility level, a routable slug, and a language.
///
/// Inherits identity and audit from <see cref="AuditableEntity"/>.
///
/// Concrete content types (Item, MediaAsset, etc.) inherit from this.
/// Non-content entities (Comment, Reaction, Subscription, etc.) inherit
/// directly from <see cref="AuditableEntity"/> instead.
/// </summary>
public abstract class ContentEntity : AuditableEntity
{
    /// <summary>Draft → Published → Archived content lifecycle.</summary>
    public ItemStatus Status { get; protected set; } = ItemStatus.Draft;

    /// <summary>
    /// UTC timestamp of when this content was first published.
    /// Null if the content has never been published.
    /// Distinct from <see cref="AuditableEntity.CreatedAt"/> — content may be
    /// created weeks before it goes live.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; private set; }

    /// <summary>
    /// ID of the user who first published this content.
    /// Null if the content has never been published.
    /// </summary>
    public string? PublishedBy { get; private set; }

    /// <summary>
    /// Controls who can see this content independent of <see cref="Status"/>.
    /// Public | Authenticated | Restricted | Private.
    /// </summary>
    public ItemVisibility Visibility { get; protected set; } = ItemVisibility.Private;

    /// <summary>
    /// URL-safe identifier unique within a content type, e.g. "my-first-blog-post".
    /// For profiles this is typically the username or handle.
    /// Null for non-routable content (e.g. social feed posts, activity items).
    /// </summary>
    public string? Slug { get; protected set; }

    /// <summary>
    /// ISO 639-1 language code for this content, e.g. "en", "fr", "de".
    /// Defaults to "en". Used for multilingual CMS scenarios.
    /// </summary>
    public string Language { get; protected set; } = "en";

    protected ContentEntity() { } // For deserialization / ORM

    protected ContentEntity(string? createdBy) : base(createdBy) { }

    /// <summary>
    /// Transitions this content to Published status and records the publish event.
    /// PublishedAt and PublishedBy are set only on first publish and never overwritten.
    /// </summary>
    public virtual void Publish(string? publishedBy = null)
    {
        Status = ItemStatus.Published;

        if (PublishedAt is null)
        {
            PublishedAt = DateTimeOffset.UtcNow;
            PublishedBy = publishedBy;
        }

        UpdateLastModified(publishedBy);
    }

    /// <summary>Transitions this content to Archived status.</summary>
    public virtual void Archive(string? modifiedBy = null)
    {
        Status = ItemStatus.Archived;
        UpdateLastModified(modifiedBy);
    }
}
