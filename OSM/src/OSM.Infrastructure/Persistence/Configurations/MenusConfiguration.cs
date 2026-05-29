using Microsoft.EntityFrameworkCore;
using OSM.Infrastructure.Identity;

namespace OSM.Infrastructure.Persistence.Configurations
{
    public class MenusConfiguration : IEntityTypeConfiguration<Menus>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Menus> builder)
        {
            builder.HasKey(x => x.MenuId);

            builder.Property(x => x.MenuId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.MenuName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.MenuShortName)
                .HasMaxLength(100);

            builder.Property(x => x.MenuType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.MenuGroup)
                .HasMaxLength(100);

            builder.Property(x => x.MenuUrl)
                .HasMaxLength(500);

            builder.Property(x => x.ExternalUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.IconClass)
                .HasMaxLength(100);

            builder.Property(x => x.BadgeText)
                .HasMaxLength(50);

            builder.Property(x => x.BadgeClass)
                .HasMaxLength(200);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.ModifiedBy)
                .HasMaxLength(100);

            builder.Property(x => x.Closable).HasDefaultValue(true);
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.HasOne(x => x.ParentMenu)
                .WithMany(x => x.ChildMenus)
                .HasForeignKey(x => x.ParentMenuId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ParentMenuId);

            builder.Property(m => m.CreatedAt).IsRequired();
            builder.Property(m => m.ModifiedAt);
        }
    }
}