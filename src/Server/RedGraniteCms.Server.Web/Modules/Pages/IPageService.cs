namespace RedGraniteCms.Server.Web.Modules.Pages;

/// <summary>
/// Contract for the page service.
/// Controllers depend on this — never on GraphQL or Item types.
/// </summary>
public interface IPageService
{
    Task<Page?> GetPageBySlugAsync(string slug);
    Task<List<Page>> GetPublishedPagesAsync(int count = 50, int skip = 0);
}
