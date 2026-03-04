using Microsoft.EntityFrameworkCore;
using RedGraniteCms.Server.Data;
using RedGraniteCms.Server.Services;

namespace RedGraniteCms.Server.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Database
        var connectionString = builder.Configuration.GetConnectionString("CosmosConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseCosmos(connectionString!, "RedGraniteCms"));

        // Application services
        builder.Services.AddRepositories();
        builder.Services.AddServices();

        // MVC
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Ensure database is created at startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapControllerRoute(
            name: "page",
            pattern: "{slug}",
            defaults: new { controller = "Page", action = "Index" });

        app.MapControllerRoute(
            name: "default",
            pattern: "",
            defaults: new { controller = "Home", action = "Index" });

        app.Run();
    }
}
