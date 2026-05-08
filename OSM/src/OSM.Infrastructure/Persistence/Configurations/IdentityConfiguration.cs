using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSM.Domain.Entities.Products;
using OSM.Infrastructure.Identity;

namespace OSM.Infrastructure.Persistence.Configurations
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TokenHash).HasMaxLength(500).IsRequired();
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
        }
    }

    public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");
            builder.HasKey(x => x.PermissionId);
            builder.Property(x => x.PermissionId).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300);

            builder.HasData(
                new Permission { PermissionId = PermissionCodes.Read, Description = "Read" },
                new Permission { PermissionId = PermissionCodes.Write, Description = "Create or update" },
                new Permission { PermissionId = PermissionCodes.Delete, Description = "Delete" });
        }
    }

    public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RoleMenuPermission>
    {
        public void Configure(EntityTypeBuilder<RoleMenuPermission> builder)
        {
            builder.ToTable("RoleMenuPermission");
            builder.HasKey(x => new { x.RoleId, x.MenuId, x.PermissionId });
            builder.HasOne(x => x.Role).WithMany(x => x.RoleMenuPermissions).HasForeignKey(x => x.RoleId);
            builder.HasOne(x => x.Permission).WithMany(x => x.RoleMenuPermissions).HasForeignKey(x => x.PermissionId);
            builder.HasOne(x => x.Menu).WithMany(x => x.RoleMenuPermissions).HasForeignKey(x => x.MenuId);
        }
    }

    public static class PermissionCodes
    {
        public const string Read = "read";
        public const string Write = "write";
        public const string Delete = "delete";

        public static readonly string[] All = [Read, Write, Delete];
    }
}
