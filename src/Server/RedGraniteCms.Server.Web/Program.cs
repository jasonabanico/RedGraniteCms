using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RedGraniteCms.Server.Data;
using RedGraniteCms.Server.Services;

namespace RedGraniteCms.Server.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Database
        builder.Services.AddDatabase(builder.Configuration, builder.Environment);

        // Application services
        builder.Services.AddRepositories();
        builder.Services.AddServices();

        // MVC
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Apply pending migrations at startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Serve React admin app static files
        var adminPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "admin");
        if (Directory.Exists(adminPath))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(adminPath),
                RequestPath = "/admin"
            });
        }

        app.UseRouting();

        // Map admin routes to React SPA (must be before other routes)
        app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/admin"), admin =>
        {
            admin.UseRouting();
            admin.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("/admin/{**path}", "/admin/index.html");
            });
        });

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
