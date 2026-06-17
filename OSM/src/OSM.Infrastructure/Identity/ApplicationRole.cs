using Microsoft.AspNetCore.Identity;

namespace OSM.Infrastructure.Identity
{
    public sealed class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
        public ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = [];
        public ICollection<RoleMenu> RoleMenus { get; set; } = [];
    }
}
