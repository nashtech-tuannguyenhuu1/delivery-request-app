namespace Core.Data;

/// <summary>
/// Abstraction over "who is acting" for auditing. Implemented at the composition root
/// (e.g. from the HTTP context). Returns null when there is no authenticated user.
/// </summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
}
