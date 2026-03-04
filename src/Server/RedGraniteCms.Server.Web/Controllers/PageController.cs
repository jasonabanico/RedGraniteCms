using Microsoft.AspNetCore.Mvc;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;
using RedGraniteCms.Server.Web.ViewModels;

namespace RedGraniteCms.Server.Web.Controllers;

/// <summary>
/// Serves individual CMS pages by slug for SEO-friendly server-side rendering.
/// </summary>
public class PageController : Controller
{
    private readonly IItemService _itemService;

    public PageController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [Route("{slug}")]
    public async Task<IActionResult> Index(string slug)
    {
        // Find the published, public item that matches this slug
        var items = await _itemService.GetItemsAsync(maxDate: null, count: null);

        var item = items.FirstOrDefault(i =>
            i.Slug == slug
            && i.Status == ItemStatus.Published
            && i.Visibility == ItemVisibility.Public);

        if (item is null)
            return NotFound();

        var model = PageViewModel.FromItem(item);
        return View(model);
    }
}
