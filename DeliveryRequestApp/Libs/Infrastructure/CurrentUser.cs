using Core.Data;

namespace Infrastructure;

/// <summary>
/// Scoped, mutable holder for the current user id. Populated once per request by
/// <see cref="CurrentUserMiddleware"/> and read by consumers through <see cref="ICurrentUser"/>.
/// </summary>
public class CurrentUser : ICurrentUser
{
    public Guid? UserId { get; set; }
}
