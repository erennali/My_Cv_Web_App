using ErenAliKocaCV.Data;
using ErenAliKocaCV.Services.Implementations;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to avoid port conflicts
builder.WebHost.ConfigureKestrel(options => {
    // Use a different port if the default one is in use
    var urls = builder.Configuration["Urls"] ?? "http://localhost:0"; // Use port 0 to auto-select an available port
    builder.WebHost.UseUrls(urls);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure caching for better performance
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

// Register HTTP client for GitHub API
builder.Services.AddHttpClient();

// Register services with their interfaces - Dependency Injection
builder.Services.AddScoped<IPersonalInfoService, PersonalInfoService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProfessionalService, ProfessionalService>();
builder.Services.AddScoped<IMediumArticleService, MediumArticleService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICVFileService, CVFileService>();
builder.Services.AddScoped<ISiteSettingsService, SiteSettingsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// Add authentication services with enhanced security
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.LogoutPath = "/Admin/Account/Logout";
        options.AccessDeniedPath = "/Admin/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        
        // Additional security configurations
        options.Cookie.HttpOnly = true; // Prevents client-side JavaScript from accessing the cookie
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Only send cookie over HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // Prevents the cookie from being sent in cross-site requests
    });

// Add compression services
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = new[] {
        "text/plain",
        "text/css",
        "application/javascript",
        "text/html",
        "application/xml",
        "text/xml",
        "application/json",
        "text/json",
        "image/svg+xml"
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Genel hata sayfası için hata yakalayıcısı
    app.UseExceptionHandler("/Home/Error");
    
    // 404 (Sayfa bulunamadı) hatası için özel sayfa
    app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");
    
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Enable compression
app.UseResponseCompression();

// Enable caching
app.UseResponseCaching();

// Add cache control headers
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = 
        context.Request.Path.StartsWithSegments("/css") || 
        context.Request.Path.StartsWithSegments("/js") || 
        context.Request.Path.StartsWithSegments("/images") || 
        context.Request.Path.StartsWithSegments("/lib") || 
        context.Request.Path.StartsWithSegments("/clark-master") 
            ? "public, max-age=604800" // Cache static resources for a week
            : "no-cache, no-store, must-revalidate"; // Don't cache dynamic content
            
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();