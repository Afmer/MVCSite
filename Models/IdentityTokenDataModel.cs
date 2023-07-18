using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MVCSite.Features.Configurations;

namespace MVCSite.Models;
[PrimaryKey(nameof(IdentityToken))]
public class IdentityTokenDataModel
{
    [StringLength(DBSettings.IdentityTokenLength - DBSettings.IdentityTokenLength % 2)]
    public string IdentityToken {get; set;}
    [StringLength(DBSettings.LoginMaxLength, MinimumLength = DBSettings.LoginMinLength)]
    public string Login {get; set;}
    public DateTime DateUpdate {get; set;}
    public IdentityTokenDataModel(string identityToken, string login)
    {
        IdentityToken = identityToken;
        Login = login;
    }
}