using Microsoft.EntityFrameworkCore;
using MVCSite.Features.Enums;
namespace MVCSite.Models;
[PrimaryKey(nameof(Login))]
public class UserIdentityDataModel
{
    public string Login {get; set;}
    public string Password {get; set;}
    public int Salt {get; set;}
    public Role Role {get; set;}
    public UserIdentityDataModel(string login, string password, int salt, Role role)
    {
        Login = login;
        Password = password;
        Salt = salt;
        Role = role;
    }
}