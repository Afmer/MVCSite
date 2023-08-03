using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MVCSite.Features.Configurations;

namespace MVCSite.Models;
[PrimaryKey(nameof(Id))]
public class RecipeDataModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id {get; set;}
    [Required]
    [StringLength(DBSettings.LabelMaxLength)]
    public string? Label {get; set;}
    [Required]
    public Guid LabelImage {get; set;}
    [Required]
    public string? AuthorLogin {get; set;}
    [Required]
    public DateTime DateOfCreation {get; set;}
    [Required]
    public string? Content {get; set;}
}