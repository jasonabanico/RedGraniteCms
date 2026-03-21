using Microsoft.AspNetCore.Mvc;
using RedGraniteCms.Server.Core.Exceptions;
using RedGraniteCms.Server.Core.Interfaces;
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
        try
        {
            var item = await _itemService.GetItemBySlugAsync(slug);
            var model = PageViewModel.FromItem(item!);
            return View(model);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
