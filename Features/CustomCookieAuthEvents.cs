using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVCSite.Features.Extensions;
using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Features;
public class CustomCookieAuthEvents : CookieAuthenticationEvents
{
    protected IDBContext _db;
    public override Task SigningOut(CookieSigningOutContext context)
    {
        var claim = context.HttpContext.User.FindFirst(c => c.Type == Constant.IdentityToken);
        string token = null!;
        if(claim != null)
        {
            token = claim.Value;
            var removalRecord = _db.IdentityTokens.Find(token);
            if(removalRecord != null)
                _db.IdentityTokens.Remove(removalRecord);
            _db.SaveChangesAsync();
        }
        return base.SigningOut(context);
    }
    public CustomCookieAuthEvents(IDBContext db)
    {
        _db = db;
    }
}