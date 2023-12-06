using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OroSmart.Data;
using OroSmart.Models;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuration of services
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(ShareResources).GetTypeInfo().Assembly.FullName);
            return factory.Create("ShareResource", assemblyName.Name);
        };
    });

builder.Services.Configure<RequestLocalizationOptions>(
    options =>
    {
        var supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("it-IT"),
        };

        options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Authentication";
        options.LoginPath = "/Login/Login";
    });

builder.Services.AddMemoryCache();
builder.Services.AddSession();

// Build the application
var app = builder.Build();

// Middleware setup and routing
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed
AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.Run();
