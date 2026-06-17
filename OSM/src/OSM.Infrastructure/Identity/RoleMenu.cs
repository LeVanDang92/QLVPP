namespace OSM.Infrastructure.Identity
{
    public sealed class RoleMenu
    {
        public Guid RoleId { get; set; }
        public string MenuId { get; set; } = string.Empty;
        public ApplicationRole Role { get; set; } = default!;
        public Menus Menu { get; set; } = default!;
    }
}
