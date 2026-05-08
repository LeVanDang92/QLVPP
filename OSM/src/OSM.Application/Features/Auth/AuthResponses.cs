namespace OSM.Application.Features.Auth
{
    public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
    public sealed record MenuPermissionResponse(
        string MenuId,
        string MenuName,
        string MenuShortName,
        string MenuType,
        string MenuGroup,
        string MenuUrl,
        string IconIndex,
        IReadOnlyCollection<string> Permissions,
        IReadOnlyCollection<string> PermissionKeys);

    public sealed record CurrentUserResponse(
        string UserId,
        string UserName,
        IReadOnlyCollection<string> Roles,
        IReadOnlyCollection<string> Permissions,
        IReadOnlyCollection<MenuPermissionResponse> Menus);
}
