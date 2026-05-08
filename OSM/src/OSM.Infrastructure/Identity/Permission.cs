namespace OSM.Infrastructure.Identity
{
    public sealed class Permission
    {
        public string PermissionId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = [];
    }
}
