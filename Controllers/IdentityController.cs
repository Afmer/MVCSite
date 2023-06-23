using Microsoft.AspNetCore.Mvc;
using MVCSite.Features.Extensions;
using MVCSite.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using MVCSite.Features.Configurations;

namespace MVCSite.Controllers;
public class IdentityController : Controller
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IDBContext _db;
    private readonly AuthLifeTimeConfiguration _authLifeTime;
    public IdentityController(ILogger<IdentityController> logger, IDBContext db, IConfiguration configuration)
    {
        _logger = logger;
        _db = db;
        var tempLifeTime = configuration.GetSection("AuthLifeTime").Get<AuthLifeTimeConfiguration>();
        if(tempLifeTime != null)
            _authLifeTime = tempLifeTime;
        else 
            throw new Exception("AuthLifeTime didn't set");
    }
    [HttpGet]
    public IActionResult Login()
    {
        if(HttpContext.User.Identity != null)
        {
            if(HttpContext.User.Identity.IsAuthenticated)
                return LocalRedirect("~/Home/Index");
        }
        else throw new Exception("User is null");
        return View();
    }
    [HttpPost]
    public async Task<IResult> Login(string login, string password)
    {
        var user = _db.UserIdentity.Find(login);
        if (user is null) return Results.Unauthorized();
        if(!HashPassword.IsPasswordValid(password, user.Salt, user.PasswordHash)) return Results.Unauthorized();
        var token = IdentityToken.Generate();
        _db.IdentityTokens.Add(new Models.IdentityTokenDataModel(token, login){DateUpdate = DateTime.UtcNow});
        await _db.SaveChangesAsync();
        var claims = new List<Claim> { new Claim(Constant.IdentityToken, token) };
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        await AuthenticationHttpContextExtensions.SignInAsync(HttpContext.Request.HttpContext, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.Add(new TimeSpan(_authLifeTime.Days, _authLifeTime.Hours, _authLifeTime.Minutes, _authLifeTime.Seconds)),
        });
        return Results.Redirect("~/Home/Index");
    }
    public async Task<IResult> Logout()
    {
        await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext.Request.HttpContext);
        return Results.Redirect("~/Identity/Login");
    }
}