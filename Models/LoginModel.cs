using System.ComponentModel.DataAnnotations;
using MVCSite.Features.Configurations;

namespace MVCSite.Models;
public class LoginModel
{
    [Display(Name = "Логин")]
    [Required(ErrorMessage = "Введите логин")]
    public string? Login {get; set;}

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    public string? Password {get; set;}
}