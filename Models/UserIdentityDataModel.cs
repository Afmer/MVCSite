using Microsoft.EntityFrameworkCore;
using Features.Enums;
namespace MVCSite.Models;
[PrimaryKey(nameof(Login))]
public class UserDataModel
{
    public string Login {get; set;}
    public string Password {get; set;}
    public string Salt {get; set;}
    public Role Role {get; set;}
    public UserDataModel(string login, string password, string salt, Role role)
    {
        Login = login;
        Password = password;
        Salt = salt;
        Role = role;
    }
}