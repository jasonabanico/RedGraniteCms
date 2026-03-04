using Microsoft.Extensions.DependencyInjection;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Data.Repositories;

namespace RedGraniteCms.Server.Data;

public static class DataExtensions
{
    public static void AddRepositories(this IServiceCollection service)
    {
        service.AddScoped<IItemRepository, ItemRepository>();
    }
}
