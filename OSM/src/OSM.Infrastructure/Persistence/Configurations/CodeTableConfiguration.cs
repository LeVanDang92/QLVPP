using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSM.Domain.Entities;

namespace OSM.Infrastructure.Persistence.Configurations
{
    public sealed class CodeTableConfiguration : IEntityTypeConfiguration<Code_Table>
    {
        public void Configure(EntityTypeBuilder<Code_Table> builder)
        {
            builder.ToTable("Code_Table");
            builder.HasKey(x => x.Table_Code);
            builder.Property(x => x.Table_Code).HasMaxLength(50);
            builder.Property(x => x.Table_Name).HasMaxLength(150);
            builder.Property(x => x.Description).HasMaxLength(250);
            builder.Property(x => x.ModifiedBy).HasMaxLength(50);
            builder.Property(x => x.CreatedBy).HasMaxLength(50);
            builder.Property(x => x.DeletedBy).HasMaxLength(50);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
