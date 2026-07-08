using Microsoft.AspNetCore.Http;

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
        if (context.Request.Headers.TryGetValue("X-UserId", out var UserIdString)
            && Guid.TryParse(UserIdString, out var value))
        {
            _currentUser.UserId = value;
        }

        await next(context);
    }
}
