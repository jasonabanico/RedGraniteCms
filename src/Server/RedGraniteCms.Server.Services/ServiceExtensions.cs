using Microsoft.Extensions.DependencyInjection;
using RedGraniteCms.Server.Core.Interfaces;

namespace RedGraniteCms.Server.Services;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection service)
    {
        service.AddScoped<IItemService, ItemService>();
    }
}
