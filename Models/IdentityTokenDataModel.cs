using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MVCSite.Models;
[PrimaryKey(nameof(IdentityToken))]
public class IdentityTokenDataModel
{
    [MaxLength(100)]
    public string IdentityToken {get; set;}
    public string Login {get; set;}
    public DateTime DateUpdate {get; set;}
    public IdentityTokenDataModel(string identityToken, string login)
    {
        IdentityToken = identityToken;
        Login = login;
    }
}