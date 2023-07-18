using System.ComponentModel.DataAnnotations;
using MVCSite.Features.Configurations;

namespace MVCSite.Models;
public class LoginModel
{
    [StringLength(DBSettings.LoginMaxLength, MinimumLength = DBSettings.LoginMinLength)]
    public string? Login {get; set;}
    [StringLength(DBSettings.PasswordMaxLength, MinimumLength = DBSettings.PasswordMinLength)]
    public string? Password {get; set;}
}