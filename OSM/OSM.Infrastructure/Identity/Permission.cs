namespace OSM.Infrastructure.Identity
{
    public sealed class Permission
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
