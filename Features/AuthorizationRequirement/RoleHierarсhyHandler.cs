using Microsoft.AspNetCore.Authorization;
using MVCSite.Interfaces;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Features.Enums;

namespace MVCSite.Features.AuthorizationRequirement;
public class RoleHierarсhyHandler : AuthorizationHandler<RoleHierarсhyRequirement>
{
    protected IDBManager _db;
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleHierarсhyRequirement requirement)
    {
        var claim = context.User.FindFirst(c => c.Type == CookieType.IdentityToken);
        string token = null!;
        if(claim != null)
            token = claim.Value;
        else
        {
            context.Fail();
            return Task.CompletedTask;
        }
        if(token != null)
        {
            var user = _db.GetUserInformationFromToken(token);
            if(user != null)
            {
                Role role = user.Role;
                if(role <= requirement.Role)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
            else context.Fail();
        }
        else context.Fail();
        return Task.CompletedTask;
    }
    public RoleHierarсhyHandler(IDBManager db) => _db = db;
}