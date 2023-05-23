using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MVCSite.Features.Enums;
namespace MVCSite.Models;
[PrimaryKey(nameof(Login))]
public class UserIdentityDataModel
{
    [MaxLength(20)]
    public string Login {get; set;}
    [MaxLength(40)]
    public string PasswordHash {get; set;}
    public int Salt {get; set;}
    public Role Role {get; set;}
    public UserIdentityDataModel(string login, string passwordHash, int salt, Role role)
    {
        Login = login;
        PasswordHash = passwordHash;
        Salt = salt;
        Role = role;
    }
}