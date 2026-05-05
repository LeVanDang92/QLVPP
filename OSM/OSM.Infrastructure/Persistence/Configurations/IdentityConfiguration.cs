using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSM.Infrastructure.Identity;

namespace OSM.Infrastructure.Persistence.Configurations
{

    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Token).HasMaxLength(500).IsRequired();
            builder.HasIndex(x => x.Token).IsUnique();
            builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
        }
    }

    public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(300);
            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasData(
                new Permission { Id = 1, Code = PermissionCodes.Read, Description = "Read" },
                new Permission { Id = 2, Code = PermissionCodes.Write, Description = "Create or update" },
                new Permission { Id = 3, Code = PermissionCodes.Delete, Description = "Delete" });
        }
    }

    public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");
            builder.HasKey(x => new { x.RoleId, x.PermissionId });
            builder.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
            builder.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId);
        }
    }

    public static class PermissionCodes
    {
        public const string Read = "read";
        public const string Write = "write";
        public const string Delete = "delete";
    }
}
