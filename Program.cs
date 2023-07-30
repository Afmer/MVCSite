using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MVCSite.Features.MariaDB;
using MVCSite.Interfaces;
using MVCSite.Features.AuthorizationRequirement;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Features.Enums;
using MVCSite.Features;
using MVCSite.Features.HostedServices;
using MVCSite.Features.Middleware;
using MVCSite.Features.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//builder.Services.ConfigureApplicationCookie(options => options.EventsType = typeof(CustomCookieAuthEvents));
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Login";
        options.AccessDeniedPath = "/accessdenied";
        options.EventsType = typeof(CustomCookieAuthEvents);
    });
builder.Services.AddScoped<CustomCookieAuthEvents>();
builder.Services.AddDbContextPool<MariaDbContext>(options => options
        .UseMySql(
            builder.Configuration.GetConnectionString("MariaDbConnectionString"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnectionString"))
        )
);
builder.Services.AddScoped<IDBContext, MariaDbContext>();
builder.Services.AddScoped<IDBManager, DbManager>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddTransient<IAuthorizationHandler, RoleHierarсhyHandler>();
builder.Services.AddAuthorization(opts => 
{
    opts.AddPolicy(PolicyName.AdminHierarchy, policy => policy.Requirements.Add(new RoleHierarсhyRequirement(Role.Admin)));
});
builder.Services.AddHostedService<IdentityTokenLifeTimeService>();

var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
    using(var dbContext = scope.ServiceProvider.GetRequiredService<MariaDbContext>())
    {
        try
        {
            dbContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

app.UseAuthentication();
app.UseAuthorization();   // добавление middleware авторизации 
app.UseMiddleware<AuthChecker>();
app.MapGet("/accessdenied", async (HttpContext context) =>
{
    context.Response.StatusCode = 403;
    await context.Response.WriteAsync("Access Denied");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
