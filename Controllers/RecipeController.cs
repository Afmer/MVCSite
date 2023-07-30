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
        throw new NotImplementedException();
    }
    [Authorize]
    [HttpPost]
    public Task<IActionResult> Edit(EditRecipeModel model)
    {
        throw new NotImplementedException();
    }
    [Authorize]
    [HttpGet] 
    public IActionResult Create()
    {
        var model = new CreateRecipeModel();
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
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRecipeModel model)
    {
        if(!ModelState.IsValid) return View(model);
        IEnumerable<TempRecipeImageInfoDataModel> imageForDelete = null!;
        Guid labelImage = Guid.Empty;
        var transactionResult = await _dbManager.ExecuteInTransaction(async () => 
        {
            var recipe = new RecipeDataModel();
            recipe.Content = model.Content;
            recipe.DateOfCreation = DateTime.UtcNow;
            recipe.Id = model.RecipeId;
            recipe.Label = model.Label;
            var imageUploadResult = await _imageService.Upload(model.LabelImage!, "LabelImages");
            if(imageUploadResult.Status != ImageUploadStatusCode.Success)
                throw new Exception("Label image upload failed");
            recipe.LabelImage = imageUploadResult.Id;
            labelImage = imageUploadResult.Id;
            var claim = HttpContext.User.FindFirst(c => c.Type == CookieType.IdentityToken);
            IdentityTokenDataModel tokenData = null!;
            if(claim != null)
            {
                var token = claim.Value;
                tokenData = _dbManager.GetIdentityToken(token);
                if(tokenData == null)
                    throw new Exception("token not defined");
            }
            else
            {
                throw new Exception("token not defined");
            }
            recipe.AuthorLogin = tokenData.Login;
            var addRecipeResult = await _dbManager.AddRecipe(recipe);
            if(!addRecipeResult)
                throw new Exception("Recipe hasn't been added in database");
            var matches = Regex.Matches(model.Content!, @"<img\s+[^>]*src=""\/api\/Image\/Show\?id=(?<id>[^&]*)&amp;imageArea=RecipeImages""[^>]*>");
            var ids = new Queue<Guid>();
            foreach(Match match in matches)
                ids.Enqueue(new Guid(match.Groups["id"].Value));
            var CheckAndMigrateResult = await _dbManager.CheckAndMigrateTempImages(ids, model.RecipeId);
            if(CheckAndMigrateResult.Status != MigrateTempImageStatusCode.Success)
                throw new Exception("Image migration failed");
            imageForDelete = CheckAndMigrateResult.ImageForDelete;
        });
        Response.Cookies.Delete(CookieType.RecipeID);
        if(transactionResult.Success)
        {
            if(imageForDelete != null)
            {
                foreach(var image in imageForDelete)
                {
                    await _imageService.Delete(image.Id, "RecipeImages");
                }
            }
            return LocalRedirect("~/Home/Index");
        }
        else
        {
            var result = await _dbManager.DeleteAllTempRecipeImages(model.RecipeId);
            if(result.Success)
                foreach(var imageId in result.DeletedImages)
                    await _imageService.Delete(imageId, "RecipeImages");
            if(labelImage != Guid.Empty)
                await _imageService.Delete(labelImage, "LabelImages");
            Console.WriteLine(transactionResult.Exception.Message);
            return Content("unknown error");
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