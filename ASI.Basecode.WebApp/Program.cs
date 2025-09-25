using System.IO;
using ASI.Basecode.WebApp;
using ASI.Basecode.WebApp.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var appBuilder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
});

appBuilder.Configuration.AddJsonFile("appsettings.json",
    optional: true,
    reloadOnChange: true);

appBuilder.WebHost.UseIISIntegration();

appBuilder.Logging
    .AddConfiguration(appBuilder.Configuration.GetLoggingSection())
    .AddConsole()
    .AddDebug();

var configurer = new StartupConfigurer(appBuilder.Configuration);
configurer.ConfigureServices(appBuilder.Services);

var app = appBuilder.Build();

configurer.ConfigureApp(app, app.Environment);

app.MapControllerRoute(
    name: "customer_default",
    pattern: "Customer/{action=Index}/{id?}",
    defaults: new { area = "Customer", controller = "Customer" });

app.MapControllerRoute(
    name: "restaurant_default",
    pattern: "Restaurant/{action=Index}/{id?}",
    defaults: new { area = "Restaurant", controller = "Restaurant" });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.MapControllers();
app.MapRazorPages();

// Run application
app.Run();