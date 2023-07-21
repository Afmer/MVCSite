using Microsoft.AspNetCore.Authentication.Cookies;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Interfaces;

namespace MVCSite.Features;
public class CustomCookieAuthEvents : CookieAuthenticationEvents
{
    protected IDBManager _db;
    public override async Task<Task> SigningOut(CookieSigningOutContext context)
    {
        var claim = context.HttpContext.User.FindFirst(c => c.Type == CookieType.IdentityToken);
        string token = null!;
        if(claim != null)
        {
            token = claim.Value;
            await _db.RemoveIdentityToken(token);
        }
        return base.SigningOut(context);
    }
    public CustomCookieAuthEvents(IDBManager db)
    {
        _db = db;
    }
}