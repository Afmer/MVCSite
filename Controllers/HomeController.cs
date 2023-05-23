using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Models;
using MVCSite.Features.MariaDB;
using MVCSite.Interfaces;

namespace MVCSite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDBContext _db;

    public HomeController(ILogger<HomeController> logger, IDBContext db)
    {
        _logger = logger;
        _db = db;
    }
    public IActionResult Index(string id = "null")
    {
        //id = WeatherForecastService.FindOne(1).Result.Id.ToString();
        ViewBag.id = "kururin";
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
