namespace RedGraniteCms.Server.Web.Modules.Pages;

/// <summary>
/// Module-level content model for a CMS page.
/// Controllers work with this — never with Item or ItemDto.
/// </summary>
public class Page
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Summary { get; init; }
    public string? Content { get; init; }
    public string? Slug { get; init; }
    public string Language { get; init; } = "en";
    public DateTimeOffset? PublishedAt { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; } = [];
    public string? MetaDescription { get; init; }
    public string? OgImageUrl { get; init; }
}
