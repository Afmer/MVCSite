using Microsoft.AspNetCore.Authorization;
using MVCSite.Interfaces;
using MVCSite.Features.Extensions;
using MVCSite.Features.Enums;

namespace MVCSite.Features.AuthorizationRequirement;
public class RoleHierarсhyHandler : AuthorizationHandler<RoleHierarсhyRequirement>
{
    protected IDBContext _db;
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleHierarсhyRequirement requirement)
    {
        var token = context.User.FindFirst(c => c.Type == Constant.IdentityToken);
        if(token != null)
        {
            var tokenRecord = _db.IdentityTokens.Find(token);
            if(tokenRecord != null)
            {
                var user = _db.UserIdentity.Find(tokenRecord.Login);
                var validUser = user ?? throw new Exception("the user was not found using the token");
                Role role = validUser.Role;
                if(role >= requirement.Role)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
            else context.Fail();
        }
        else context.Fail();
        return Task.CompletedTask;
    }
    public RoleHierarсhyHandler(IDBContext db) => _db = db;
}