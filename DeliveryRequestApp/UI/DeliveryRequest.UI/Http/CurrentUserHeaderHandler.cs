using System.Security.Claims;

namespace DeliveryRequest.UI.Http;

/// <summary>
/// Attaches the signed-in user's id (from the auth cookie) as an "X-UserId" header on every
/// outgoing request to the DeliveryRequest API, so the API's CurrentUserMiddleware/ICurrentUser
/// can identify who is acting for auditing purposes.
/// </summary>
public class CurrentUserHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            request.Headers.Remove("X-UserId");
            request.Headers.Add("X-UserId", userId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
