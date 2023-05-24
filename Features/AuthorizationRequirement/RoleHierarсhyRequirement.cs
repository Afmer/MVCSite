using Microsoft.AspNetCore.Authorization;
using MVCSite.Features.Enums;

namespace MVCSite.Features.AuthorizationRequirement;
public class RoleHierarсhyRequirement : IAuthorizationRequirement
{
    public Role Role {get; set;}
    public RoleHierarсhyRequirement(Role role) => Role = role;
}