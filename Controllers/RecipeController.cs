using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVCSite.Controllers;
public class RecipeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    [Authorize]
    public IActionResult Edit()
    {
        return View();
    }
}