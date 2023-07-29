using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Enums;
using MVCSite.Features.Extensions.Constants;
using Microsoft.AspNetCore.Authorization;

namespace MVCSite.Areas.api.Controllers;
[Area("api")]
public class ImageController : Controller
{
    private readonly string _hostEnviroment;
    private readonly IDBManager _db;
    private readonly IImageService _imageService;
    public ImageController(IWebHostEnvironment hostEnvironment, IDBManager db, IImageService imageService)
    {
        _hostEnviroment = hostEnvironment.ContentRootPath;
        _db = db;
        _imageService = imageService;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadRecipeImage()
    {
        var uploadedFile = Request.Form.Files.FirstOrDefault();
        if(uploadedFile == null)
            return Content("{\"status\"=\"error\"}", "application/json");
        string recipeIdStr = Request.Cookies[CookieType.RecipeID]!;
        if(recipeIdStr == null)
            return Content("{\"status\"=\"error\"}", "application/json");
        Guid recipeId = new Guid(Request.Cookies[CookieType.RecipeID]!);
        var imageUploadResult = await _imageService.Upload(uploadedFile, "RecipeImages");
        if(imageUploadResult.Status == ImageUploadStatusCode.Success)
        {
            var image = new TempRecipeImageInfoDataModel();
            image.DateOfCreation = DateTime.UtcNow;
            image.Id = imageUploadResult.Id;
            image.RecipeId = recipeId;
            if((await _db.AddTempRecipeImage(image)) == AddTempRecipeImageStatusCode.Error)
            {
                throw new Exception();
            }
            var data = new { url = imageUploadResult.Url  };
            return new JsonResult(data);
        }
        else
        {
            return Content("{\"status\"=\"error\"}", "application/json");
        }
    }
    [HttpGet]
    public IActionResult Show(Guid id, string imageArea)
    {
        try
        {
            var path = _hostEnviroment + $"/AppData/{imageArea}/" + id.ToString() + ".jpg";
            FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);
            return File(fileStream, "image/jpeg");
        }
        catch
        {
            return NotFound();
        }
    }
}