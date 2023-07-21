using System.ComponentModel.DataAnnotations;
using MVCSite.Features.Configurations;

namespace MVCSite.Models;
public class RegisterModel
{
    [Display(Name = "Логин")]
    [Required(ErrorMessage = "Введите логин")]
    [StringLength(DBSettings.LoginMaxLength, MinimumLength = DBSettings.LoginMinLength, ErrorMessage = "Логин должен быть от {2} до {1} символов")]
    public string? Login {get; set;}

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [StringLength(DBSettings.PasswordMaxLength, MinimumLength = DBSettings.PasswordMinLength, ErrorMessage = "Пароль должен быть от {2} до {1} символов")]
    public string? Password {get; set;}

    [Display(Name = "Подтвердите пароль")]
    [Required(ErrorMessage = "Введите пароль повторно")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Введите электронную почту")]
    [EmailAddress(ErrorMessage = "Некорректная электронная почта")]
    public string? Email {get; set;}
}