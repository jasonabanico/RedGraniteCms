using Microsoft.AspNetCore.Mvc;
using RedGraniteCms.Server.Web.Modules.Pages;
using RedGraniteCms.Server.Web.ViewModels;

namespace RedGraniteCms.Server.Web.Controllers;

/// <summary>
/// Serves the site home page — lists all published pages.
/// </summary>
public class HomeController : Controller
{
    private readonly IPageModule _pageModule;

    public HomeController(IPageModule pageModule)
    {
        _pageModule = pageModule;
    }

    public async Task<IActionResult> Index()
    {
        var pages = await _pageModule.GetPublishedPagesAsync(count: 50);

        var model = new HomeViewModel
        {
            Pages = pages
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("Home/Error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
