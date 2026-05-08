using OSM.Domain.Common;

namespace OSM.Infrastructure.Identity
{
    public class Menus : IAuditableEntity
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuShortName { get; set; }
        public string MenuType { get; set; }
        public string MenuGroup { get; set; }
        public string? MenuUrl { get; set; } 
        public string IconIndex { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        public ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = [];
    }
}
