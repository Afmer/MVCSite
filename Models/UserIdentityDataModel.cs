using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MVCSite.Settings;
using MVCSite.Features.Enums;
namespace MVCSite.Models;
[PrimaryKey(nameof(Login))]
public class UserInformationDataModel
{
    [StringLength(DBSettings.LoginMaxLength, MinimumLength = DBSettings.LoginMinLength)]
    public string Login {get; set;}
    [MaxLength(40)]
    public string PasswordHash {get; set;}
    public int Salt {get; set;}
    public Role Role {get; set;}
    [EmailAddress]
    public string Email {get; set;}
    public UserInformationDataModel(string login, string passwordHash, int salt, Role role, string email)
    {
        Login = login;
        PasswordHash = passwordHash;
        Salt = salt;
        Role = role;
        Email = email;
    }
}