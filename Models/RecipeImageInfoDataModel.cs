using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MVCSite.Models;
[PrimaryKey(nameof(Id))]
public class RecipeImageInfoDataModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id {get; set;}
    [Required]
    public DateTime DateOfCreation {get; set;}
    [Required]
    public Guid? RecipeId {get; set;}
    public virtual RecipeDataModel? Recipe {get; set;} = null!;
}