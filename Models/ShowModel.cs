using System.ComponentModel.DataAnnotations;

namespace MVCSite.Models;
public class ShowModel
{
    [Required]
    public string Content {get; set;} = null!;
    [Required]
    public string Label {get; set;} = null!;
    [Required]
    public string LabelImageLink {get; set;} = null!;
}