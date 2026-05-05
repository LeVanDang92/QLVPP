namespace OSM.Application.Abstractions.Authentication
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
    }
}
