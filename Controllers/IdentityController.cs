using Microsoft.AspNetCore.Mvc;
using MVCSite.Features.MariaDB;
using MVCSite.Features.Extensions;
namespace MVCSite.Controllers;
public class IdentityController : Controller
{
    private readonly ILogger<IdentityController> _logger;
    private readonly MariaDbContext _db;
    public IdentityController(ILogger<IdentityController> logger, MariaDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public IResult Login(string login, string password)
    {
        var user = _db.UserIdentity.Find(login);
        if (user is null) return Results.Unauthorized();
        if(!HashPassword.IsPasswordValid(password, user.Salt, user.Password)) return Results.Unauthorized();
        return Results.Redirect("~/Home/Index");
    }
}