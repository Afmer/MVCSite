using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;
namespace MVCSite.Areas.api.Controllers;
[Area("api")]
public class ImageController : Controller
{
    private readonly string _hostEnviroment;
    private readonly IDBContext _db;
    public ImageController(IWebHostEnvironment hostEnvironment, IDBContext db)
    {
        _hostEnviroment = hostEnvironment.ContentRootPath;
        _db = db;
    }
    [HttpPost]
    public async Task<IActionResult> Upload()
    {
        var uploadedFile = Request.Form.Files.FirstOrDefault();
        if(uploadedFile == null)
            return Content("{}", "application/json");
        var image = new RecipeImageInfoDataModel();
        var addImageResult = await _db.AddRecipeImage(image);
        if(addImageResult.status == Features.Enums.AddRecipeImageStatusCode.Success)
        {
            var imagePath = _hostEnviroment + "/AppData/RecipeImages/" + addImageResult.imageId.ToString() + ".jpg";
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }
            var data = new { url = "/api/Image/Show?" + "id=" + image.Id.ToString()  };
            var json = JsonSerializer.Serialize(data);
            Console.WriteLine(json);
            var a = new JsonResult(data);
            return new JsonResult(data);
        }
        else
        {
            return Content("{\"status\"=\"error\"}", "application/json");
        }
    }
    [HttpGet]
    public IActionResult Show(Guid id)
    {
        try
        {
            var path = _hostEnviroment + "/AppData/RecipeImages/" + id.ToString() + ".jpg";
            FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);
            return File(fileStream, "image/jpeg");
        }
        catch
        {
            return NotFound();
        }
    }
}