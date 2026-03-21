using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Data.Repositories;

namespace RedGraniteCms.Server.Data;

public static class DataExtensions
{
    public static void AddRepositories(this IServiceCollection service)
    {
        service.AddScoped<IItemRepository, ItemRepository>();
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            if (environment.IsDevelopment())
            {
                var sqliteConnection = configuration.GetConnectionString("DefaultConnection")
                    ?? "Data Source=RedGraniteCms.db";
                options.UseSqlite(sqliteConnection);
            }
            else
            {
                var pgConnection = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                options.UseNpgsql(pgConnection)
                       .UseSnakeCaseNamingConvention();
            }
        });
    }
}
