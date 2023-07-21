using Microsoft.AspNetCore.Mvc;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Features.Extensions;
using MVCSite.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MVCSite.Features.Configurations;
using MVCSite.Models;
using MVCSite.Features.Enums;

namespace MVCSite.Controllers;
public class IdentityController : Controller
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IDBManager _db;
    private readonly AuthLifeTimeConfiguration _authLifeTime;
    public IdentityController(ILogger<IdentityController> logger, IDBManager db, IConfiguration configuration)
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
    public async Task<IActionResult> Login(LoginModel model)
    {
        if(ModelState.IsValid)
        {
            var login = model.Login;
            var password = model.Password;
            if(login == null || password == null)
                return Content("unkonwn error");
            var loginResult = await _db.LoginHandler(login, password);
            if(loginResult.status == LoginStatusCode.LoginOrPasswordError)
            {
                ModelState.AddModelError(ModelValidateErrorName.LoginOrPasswordError, "Неправильный логин или пароль");
                return View(model);
            }
            else if(loginResult.status == LoginStatusCode.Success)
            {
                var claims = new List<Claim> { new Claim(CookieType.IdentityToken, loginResult.token) };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await AuthenticationHttpContextExtensions.SignInAsync(HttpContext.Request.HttpContext, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(new TimeSpan(_authLifeTime.Days, _authLifeTime.Hours, _authLifeTime.Minutes, _authLifeTime.Seconds)),
                });
                return Redirect("~/Home/Index");
            }
            else
            {
                return Content("unkonwn error");
            }
        }
        else
        {
            return View(model);
        }
    }
    public async Task<IResult> Logout()
    {
        await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext.Request.HttpContext);
        return Results.Redirect("~/Identity/Login");
    }
    [HttpGet]
    public IActionResult Register()
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
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if(ModelState.IsValid)
        {
            if(model.Login == null || model.Password == null || model.Email == null)
                return Content("unkonwn error");
            var salt = HashPassword.GenerateSaltForPassword();
            var hash = HashPassword.ComputePasswordHash(model.Password, salt);
            var userDataModel = new UserInformationDataModel(model.Login, hash, salt, Role.User, model.Email);
            var registerResult = await _db.RegisterHandler(userDataModel);
            if(registerResult.status == RegisterStatusCode.Success)
            {
                var claims = new List<Claim> { new Claim(CookieType.IdentityToken, registerResult.token) };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await AuthenticationHttpContextExtensions.SignInAsync(HttpContext.Request.HttpContext, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(new TimeSpan(_authLifeTime.Days, _authLifeTime.Hours, _authLifeTime.Minutes, _authLifeTime.Seconds)),
                });
                return Redirect("~/Home/Index");
            }
            else
            {
                return Content("unkonwn error");
            }
        }
        else
        {
            return View(model);
        }
    }
}