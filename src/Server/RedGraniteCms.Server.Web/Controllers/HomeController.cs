using Microsoft.AspNetCore.Mvc;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;
using RedGraniteCms.Server.Web.ViewModels;

namespace RedGraniteCms.Server.Web.Controllers;

/// <summary>
/// Serves the site home page — lists all published pages.
/// </summary>
public class HomeController : Controller
{
    private readonly IItemService _itemService;

    public HomeController(IItemService itemService)
    {
        _itemService = itemService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _itemService.GetItemsAsync(maxDate: null, count: null);

        var pages = items
            .Where(i => i.Status == ItemStatus.Published
                     && i.Visibility == ItemVisibility.Public)
            .OrderBy(i => i.SortOrder)
            .ThenByDescending(i => i.PublishedAt)
            .Select(PageViewModel.FromItem)
            .ToList();

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
