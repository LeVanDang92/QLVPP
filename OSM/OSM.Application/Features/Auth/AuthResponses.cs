namespace OSM.Application.Features.Auth
{
    public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
    public sealed record CurrentUserResponse(string UserId, string UserName, IReadOnlyCollection<string> Roles, IReadOnlyCollection<string> Permissions);
}
