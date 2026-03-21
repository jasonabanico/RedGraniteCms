using Microsoft.AspNetCore.Mvc;
using RedGraniteCms.Server.Web.Modules.Pages;

namespace RedGraniteCms.Server.Web.Controllers;

/// <summary>
/// Serves individual CMS pages by slug for SEO-friendly server-side rendering.
/// </summary>
public class PageController : Controller
{
    private readonly IPageService _pageService;

    public PageController(IPageService pageService)
    {
        _pageService = pageService;
    }

    [Route("pages/{slug}")]
    public async Task<IActionResult> Index(string slug)
    {
        var page = await _pageService.GetPageBySlugAsync(slug);

        if (page is null)
            return NotFound();

        return View(page);
    }
}
