namespace OSM.Application.Features.Auth
{
    public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
    public sealed record MenuPermissionResponse(
        string Id, // menu id
        string MenuName,
        string Title, // short name
        string MenuType,
        string MenuGroup,
        string Path,
        string ExternalUrl,
        string Icon,
        int DisplayOrder,
        bool Closable,
        string? ParentMenuId,
        MenuBadgeDto? Badge,
        IReadOnlyCollection<MenuPermissionResponse> Children,
        IReadOnlyCollection<string> Permissions,    //  {x.PermissionId}
        IReadOnlyCollection<string> PermissionKeys); // {x.MenuId}.{x.PermissionId} => quyền trên từng màn hình ví dụ : dashboard.read, dashboard.write

    public sealed record CurrentUserResponse(
        string UserId,
        string UserName,
        IReadOnlyCollection<string> Roles,
        IReadOnlyCollection<string> Permissions, //== PermissionKeys
        IReadOnlyCollection<MenuSection> Menus);

    public sealed record MenuBadgeDto(
        string text,
        string? className);

    public sealed record MenuSection
    (
         string Title ,
        IReadOnlyCollection<MenuPermissionResponse> Items
    );
}
