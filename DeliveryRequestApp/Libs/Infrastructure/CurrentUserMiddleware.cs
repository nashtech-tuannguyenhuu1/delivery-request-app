using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure;

/// <summary>
/// Reads the authenticated principal's NameIdentifier claim and populates the scoped
/// <see cref="CurrentUser"/> holder for the duration of the request.
/// </summary>
public class CurrentUserMiddleware : IMiddleware
{
    private readonly CurrentUser _currentUser;

    public CurrentUserMiddleware(CurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var value = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(value, out var id))
        {
            _currentUser.UserId = id;
        }

        await next(context);
    }
}
