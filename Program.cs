using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CEBlog.Data;
using CEBlog.Helpers;
using CEBlog.Models;
using CEBlog.Services; 

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
//var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//var connectionString = builder.Configuration.GetSection("pgSettings")["pgConnection"];
var connectionString = ConnectionHelper.GetConnectionString(configuration);

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddAuthentication().AddTwitter(twitterOptions =>
{
    twitterOptions.ConsumerKey = configuration["Authentication:Twitter:ConsumerAPIKey"] ?? Environment.GetEnvironmentVariable("ConsumerAPIKey");
    twitterOptions.ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"] ?? Environment.GetEnvironmentVariable("ConsumerSecret");
});

services.AddControllersWithViews();

services.AddRazorPages();

//register custom services
services.AddScoped<DataService>();
services.AddScoped<BlogSearchService>();

//register a preconfigured instance of the MailSettings class
services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
services.AddScoped<IBlogEmailSender, EmailService>();

//Register our Image Service
services.AddScoped<IImageService, BasicImageService>();

//Register the Slug Service
services.AddScoped<ISlugService, BasicSlugService>();

//Register the Slug Service
services.AddScoped<INavigationService, PostNavigationService>();

//Register Memory Cache
services.AddMemoryCache();

var app = builder.Build();
var scope = app.Services.CreateScope();

//pull out registered DataService class
var dataService = scope.ServiceProvider.GetService<DataService>();

//first, get the database update with the latest migrations
await DataHelper.ManageDataAsync(scope.ServiceProvider);

//add users and user roles into the system
await dataService.ManageDataAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/StatusCodeError/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "SlugRoute",
        pattern: "/{slug}",
        defaults: new { controller = "Posts", action = "Details" });

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();
