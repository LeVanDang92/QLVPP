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

        /// <summary>
        /// Dùng cho link bên ngoài app Angular.
        /// </summary>
        public string? ExternalUrl { get; set; } 
        public string IconClass { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Dùng cho chức năng tab. Tab có thể đóng bằng nút x.
        /// </summary>
        public bool Closable { get; set; } = true;

        /// <summary>
        /// Dùng để gắn nhãn cho menu
        /// </summary>
        public string? BadgeText { get; set; }
        public string? BadgeClass { get; set; }

        /// <summary>
        /// Menu cha
        /// </summary>
        public string? ParentMenuId { get; set; }
        public Menus? ParentMenu { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = [];
        public ICollection<RoleMenu> RoleMenus { get; set; } = [];
        public ICollection<Menus> ChildMenus { get; set; } = [];
    }
}
