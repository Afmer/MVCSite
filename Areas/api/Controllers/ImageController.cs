using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;
namespace MVCSite.Areas.api.Controllers;
[Area("api")]
public class ImageController : Controller
{
    private readonly string _hostEnviroment;
    public ImageController(IWebHostEnvironment hostEnvironment, IDBManager db)
    {
        _hostEnviroment = hostEnvironment.ContentRootPath;
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