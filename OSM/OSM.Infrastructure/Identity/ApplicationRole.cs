using Microsoft.AspNetCore.Identity;

namespace OSM.Infrastructure.Identity
{
    public sealed class ApplicationRole : IdentityRole<Guid>
    {
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
