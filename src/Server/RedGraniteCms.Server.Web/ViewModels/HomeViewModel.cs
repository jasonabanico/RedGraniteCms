using RedGraniteCms.Server.Web.Modules.Pages;

namespace RedGraniteCms.Server.Web.ViewModels;

/// <summary>
/// View model for the site home page — lists all published pages.
/// </summary>
public class HomeViewModel
{
    public string SiteName { get; init; } = "RedGraniteCms";
    public List<Page> Pages { get; init; } = new();
}
