using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MVCSite.Models;
[PrimaryKey(nameof(Id))]
public class TempRecipeImageInfoDataModel
{
    public Guid Id {get; set;}
    [Required]
    public DateTime DateOfCreation {get; set;}
    [Required]
    public Guid? RecipeId {get; set;}
}