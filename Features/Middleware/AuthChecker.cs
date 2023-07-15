using Microsoft.AspNetCore.Authentication;
using MVCSite.Features.Extensions;
using MVCSite.Interfaces;
namespace MVCSite.Features.Middleware;
public class AuthChecker
{
    private readonly RequestDelegate _next;

    public AuthChecker(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context, IDBManager dbManager)
    {
        // Обработка удаления пользователя из таблицы IdentityTokens
        if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
        {
            var claim = context.User.FindFirst(c => c.Type == Constant.IdentityToken);
            string token = null!;
            if(claim != null)
            {
                token = claim.Value;
                var tokenData = dbManager.GetIdentityToken(token);
                if(tokenData == null)
                    await AuthenticationHttpContextExtensions.SignOutAsync(context);
            }
            else
            {
                await AuthenticationHttpContextExtensions.SignOutAsync(context);
            }
        }

        // Передача запроса следующему Middleware в цепочке
        await _next(context);
    }
}