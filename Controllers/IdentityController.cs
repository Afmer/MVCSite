using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Features.MariaDB;
namespace MVCSite.Controllers;
public class IdentityController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MariaDbContext _db;
    public IdentityController(ILogger<HomeController> logger, MariaDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}