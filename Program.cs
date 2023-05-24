using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MVCSite.Features.MariaDB;
using MVCSite.Interfaces;
using MVCSite.Features.AuthorizationRequirement;
using MVCSite.Features.Extensions;
using MVCSite.Features.Enums;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Login";
        options.AccessDeniedPath = "/accessdenied";
    });
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddDbContextPool<MariaDbContext>(options => options
        .UseMySql(
            builder.Configuration.GetConnectionString("MariaDbConnectionString"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnectionString"))
        )
);
builder.Services.AddTransient<IDBContext, MariaDbContext>();
builder.Services.AddTransient<IAuthorizationHandler, RoleHierarсhyHandler>();
builder.Services.AddAuthorization(opts => 
{
    opts.AddPolicy(Constant.AdminHierarchy, policy => policy.Requirements.Add(new RoleHierarсhyRequirement(Role.Admin)));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();   // добавление middleware авторизации 

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
