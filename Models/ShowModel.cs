using System.ComponentModel.DataAnnotations;

namespace MVCSite.Models;
public class ShowModel
{
    [Required]
    public string Content {get; set;} = null!;
}