namespace RedGraniteCms.Server.Web.Modules.Pages;

/// <summary>
/// Contract for the Page module.
/// Controllers depend on this — never on GraphQL or Item types.
/// </summary>
public interface IPageModule
{
    Task<Page?> GetPageBySlugAsync(string slug);
    Task<List<Page>> GetPublishedPagesAsync(int count = 50, int skip = 0);
}
