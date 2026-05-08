namespace OSM.Infrastructure.Identity
{
    public sealed class RoleMenuPermission
    {
        public Guid RoleId { get; set; }
        public string PermissionId { get; set; } = string.Empty;
        public string MenuId { get; set; } = string.Empty;

        public ApplicationRole Role { get; set; } = default!;
        public Permission Permission { get; set; } = default!;
        public Menus Menu { get; set; } = default!;

    }
}
