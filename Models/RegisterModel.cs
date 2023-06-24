using System.ComponentModel.DataAnnotations;

namespace MVCSite.Models;
public class RegisterModel
{
    [MaxLength(20)]
    public string? Login {get; set;}
    public string? Password {get; set;}
    [EmailAddress]
    public string? Email {get; set;}
}