using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Features.Enums;

namespace MVCSite.Controllers;
public class RecipeController : Controller
{
    private readonly IDBManager _dbManager;
    private readonly IImageService _imageService;
    public RecipeController(IDBManager dbManager, IImageService imageService)
    {
        _dbManager = dbManager;
        _imageService = imageService;
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
            var cookieOptions = new CookieOptions
            {
                // Устанавливаем время жизни куки (опционально)
                Expires = DateTime.Now.AddHours(24) // Куки будет действительно в течение 1 часа
            };

            // Устанавливаем значение куки
            Response.Cookies.Append(CookieType.RecipeID, model.RecipeId.ToString(), cookieOptions);
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
    public async Task<IActionResult> Edit(EditRecipeModel model)
    {
        if(!ModelState.IsValid) return View(model);
        var recipe = new RecipeDataModel();
        recipe.Content = model.Content;
        recipe.DateOfCreation = DateTime.UtcNow;
        recipe.Id = model.RecipeId;
        recipe.Label = model.Label;
        var claim = HttpContext.User.FindFirst(c => c.Type == CookieType.IdentityToken);
        IdentityTokenDataModel tokenData = null!;
        if(claim != null)
        {
            var token = claim.Value;
            tokenData = _dbManager.GetIdentityToken(token);
            if(tokenData == null)
                return Content("unknown error");
        }
        else
        {
            return Content("unknown error");
        }
        recipe.AuthorLogin = tokenData.Login;
        var addRecipeResult = await _dbManager.AddRecipe(recipe);
        var matches = Regex.Matches(model.Content!, @"<img\s+[^>]*src=""\/api\/Image\/Show\?id=(?<id>[^&]*)&amp;imageArea=RecipeImages""[^>]*>");
        Response.Cookies.Delete(CookieType.RecipeID);
        var ids = new Queue<Guid>();
        foreach(Match match in matches)
            ids.Enqueue(new Guid(match.Groups["id"].Value));
        if(!addRecipeResult)
            return Content("unknown error");
        var CheckAndMigrateResult = await _dbManager.CheckAndMigrateTempImages(ids, model.RecipeId);
        if(CheckAndMigrateResult.Status != MigrateTempImageStatusCode.Success)
            return Content("unknown error");
        foreach(var image in CheckAndMigrateResult.ImageForDelete)
        {
            await _imageService.Delete(image.Id, "RecipeImages");
        }
        return LocalRedirect("~/Home/Index");
    }
    [HttpGet]
    public IActionResult Show(Guid id)
    {
        var recipe = _dbManager.GetRecipe(id);
        if(recipe != null && recipe.Content != null)
            return View(new ShowModel(){Content = recipe.Content});
        else
            return Content("");
    }
}