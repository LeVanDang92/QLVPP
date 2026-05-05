namespace OSM.Infrastructure.Identity
{
    public sealed class RolePermission
    {
        public Guid RoleId { get; set; }
        public ApplicationRole Role { get; set; } = default!;
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;
    }
}
