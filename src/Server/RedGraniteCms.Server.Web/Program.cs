using Microsoft.Extensions.FileProviders;
using RedGraniteCms.Server.Web.Modules.Pages;

namespace RedGraniteCms.Server.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services — each service encapsulates API communication for a content type
        var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5034/";
        builder.Services.AddHttpClient<IPageService, PageService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        // Reverse proxy HttpClient for dev SPA proxy
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHttpClient("SpaProxy", client =>
            {
                client.BaseAddress = new Uri("http://localhost:3000");
            });
        }

        // MVC
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Admin SPA: proxy to Vite in dev, serve static build in production
        var adminPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "admin");
        if (app.Environment.IsDevelopment())
        {
            // Proxy /admin requests to the Vite dev server
            app.Map("/admin/{**catch-all}", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
            {
                var client = httpClientFactory.CreateClient("SpaProxy");

                // Rewrite path: /admin/foo → /foo (Vite serves from root)
                var targetPath = context.Request.Path.Value?.Replace("/admin", "") ?? "/";
                if (string.IsNullOrEmpty(targetPath)) targetPath = "/";
                var targetUrl = targetPath + context.Request.QueryString;

                var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);

                // Forward request body for POST etc.
                if (context.Request.ContentLength > 0 || context.Request.ContentType != null)
                {
                    requestMessage.Content = new StreamContent(context.Request.Body);
                    if (context.Request.ContentType != null)
                        requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(context.Request.ContentType);
                }

                try
                {
                    var response = await client.SendAsync(requestMessage);
                    context.Response.StatusCode = (int)response.StatusCode;

                    foreach (var header in response.Content.Headers)
                        context.Response.Headers[header.Key] = header.Value.ToArray();

                    // Remove transfer-encoding since we're buffering
                    context.Response.Headers.Remove("transfer-encoding");

                    await response.Content.CopyToAsync(context.Response.Body);
                }
                catch (HttpRequestException)
                {
                    // Vite dev server not running — show a helpful message
                    context.Response.StatusCode = 502;
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(
                        "<h1>Admin UI Unavailable</h1>" +
                        "<p>The Vite dev server is not running. Start it with:</p>" +
                        "<pre>cd src/Client/RedGraniteCms.Client.Web &amp;&amp; npm run dev</pre>");
                }
            });
        }
        else
        {
            // Production: serve pre-built React app from wwwroot/admin
            if (Directory.Exists(adminPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(adminPath),
                    RequestPath = "/admin"
                });
            }

            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/admin"), admin =>
            {
                admin.UseRouting();
                admin.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToFile("/admin/{**path}", "/admin/index.html");
                });
            });
        }

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
