using Microsoft.EntityFrameworkCore;
using Features.Enums;
namespace MVCSite.Models;
[PrimaryKey(nameof(Login))]
public class UserDataModel
{
    public string Login {get; set;}
    public string Name {get; set;}
    public string Password {get; set;}
    public Role Role {get; set;}
    public UserDataModel(string name, string login, string password, Role role)
    {
        Name = name;
        Login = login;
        Password = password;
        Role = role;
    }
}