using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Controllers;
public class RecipeController : Controller
{
    private IDBManager _dbManager;
    public RecipeController(IDBManager dbManager)
    {
        _dbManager = dbManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [Authorize]
    [HttpGet]
    public IActionResult Edit(string id = "")
    {
        if(id == "")
        {
            var model = new EditRecipeModel();
            model.RecipeId = Guid.NewGuid();
            return View(model);
        }
        else if(_dbManager.IsHasRecipe(new Guid(id)))
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    [Authorize]
    [HttpPost]
    public IActionResult Edit(EditRecipeModel model)
    {
        Regex img = new(@"<img\s+[^>]*src=""(?<Link>[^""]*)""[^>]*>");
        throw new NotImplementedException();
    }
}