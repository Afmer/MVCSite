using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MVCSite.Models;
public class CreateRecipeModel
{
    [Display(Name = "Оглавление")]
    [Required]
    public string? Label {get; set;}

    [Display(Name = "Обложка")]
    [Required]
    public IFormFile? LabelImage {get; set;}

    [Display(Name = "Содержимое")]
    [Required]
    public string? Content {get; set;}
    [HiddenInput(DisplayValue = false)]
    [Required]
    public Guid RecipeId {get; set;}
}