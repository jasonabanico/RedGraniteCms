using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.Web.ViewModels;

/// <summary>
/// View model for rendering a single CMS page.
/// Maps from the server-side <see cref="Item"/> model.
/// </summary>
public class PageViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Summary { get; init; }
    public string? Content { get; init; }
    public string? Slug { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public DateTimeOffset? PublishedAt { get; init; }
    public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();

    /// <summary>SEO meta description, pulled from Metadata["seo"]["description"] if present.</summary>
    public string? MetaDescription { get; init; }

    /// <summary>Open Graph image URL, pulled from Metadata["og"]["image"] if present.</summary>
    public string? OgImageUrl { get; init; }

    public static PageViewModel FromItem(Item item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Summary = item.Summary,
        Content = item.Content,
        Slug = item.Slug,
        ContentType = item.ContentType,
        Language = item.Language,
        PublishedAt = item.PublishedAt,
        Tags = item.Tags,
        MetaDescription = item.GetMetadata<System.Text.Json.Nodes.JsonObject>("seo")?["description"]?.GetValue<string>(),
        OgImageUrl = item.GetMetadata<System.Text.Json.Nodes.JsonObject>("og")?["image"]?.GetValue<string>()
    };
}
