﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Models;
using MVCSite.Interfaces;

namespace MVCSite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDBManager _db;

    public HomeController(ILogger<HomeController> logger, IDBManager db)
    {
        _logger = logger;
        _db = db;
    }
    public IActionResult Index()
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
