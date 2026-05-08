using Microsoft.EntityFrameworkCore;
using OSM.Infrastructure.Identity;

namespace OSM.Infrastructure.Persistence.Configurations
{
    public class MenusConfiguration : IEntityTypeConfiguration<Menus>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Menus> builder)
        {
            builder.HasKey(m => m.MenuId);
            builder.Property(m => m.MenuId).IsRequired().HasMaxLength(50);
            builder.Property(m => m.MenuName).IsRequired().HasMaxLength(150);
            builder.Property(m => m.MenuShortName).HasMaxLength(100);
            builder.Property(m => m.MenuType).HasMaxLength(10);
            builder.Property(m => m.MenuGroup).HasMaxLength(10);
            builder.Property(m => m.MenuUrl).HasMaxLength(200);
            builder.Property(m => m.IconIndex).HasMaxLength(50);
            builder.Property(m => m.CreatedAt).IsRequired();
            builder.Property(m => m.CreatedBy).HasMaxLength(50);
            builder.Property(m => m.ModifiedAt);
            builder.Property(m => m.ModifiedBy).HasMaxLength(50);
        }
    }
}