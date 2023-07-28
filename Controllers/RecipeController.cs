using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Exceptions;
using MVCSite.Features.Enums;
using MVCSite.Features.Extensions.Constants;

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
        var images = new Queue<RecipeImageInfoDataModel>();
        MatchEvaluator MatchFunc = match => 
        {
            if(match.Groups["type"].Value != "image") throw new ImageTypeException("Type must be image");
            if(match.Groups["extension"].Value != "jpeg") throw new ImageTypeException("Extension must be jpeg");
            var imageUpload = _imageService.Upload(match.Groups["base64"].Value, "RecipeImages");
            if(imageUpload.Result.Status == ImageUploadStatusCode.Success)
            {
                var imageEntry = new RecipeImageInfoDataModel();
                imageEntry.DateOfCreation = DateTime.UtcNow;
                imageEntry.Id = imageUpload.Result.Id;
                imageEntry.RecipeId = model.RecipeId;
                images.Enqueue(imageEntry);
                return @$"<img {match.Groups["leftContent"]} src=""{imageUpload.Result.Url}"" {match.Groups["rightContent"]}/>";
            }
            else
                return "";
        };
        try
        {
            var claim = HttpContext.User.FindFirst(c => c.Type == CookieType.IdentityToken);
            string token = null!;
            if(claim != null)
            {
                var recipeModel = new RecipeDataModel();
                recipeModel.Id = model.RecipeId;
                string content = Regex.Replace(model.Content!, 
                @"<img(?<leftContent>\s+[^>]*)src=""(?<base64>data:(?<type>[a-zA-Z]*)\/(?<extension>[a-zA-Z]*);base64,[^""]*)(?<rightContent>[^""]*""[^>]*)>", MatchFunc);
                recipeModel.Content = content;
                recipeModel.Label = model.Label;
                recipeModel.DateOfCreation = DateTime.UtcNow;
                token = claim.Value;
                var tokenData = _dbManager.GetIdentityToken(token);
                recipeModel.AuthorLogin = tokenData.Login;
                if((await _dbManager.AddRecipe(recipeModel, images)) !=  AddRecipeStatusCode.Success)
                    return Content("unknown error");
                return LocalRedirect("~/Home/Index");
            }
            else
                return Content("");
        }
        catch(ImageTypeException e)
        {
            return Content(e.Message);
        }
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