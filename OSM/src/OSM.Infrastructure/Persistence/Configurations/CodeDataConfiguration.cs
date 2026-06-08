using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSM.Domain.Entities;

namespace OSM.Infrastructure.Persistence.Configurations
{
    public sealed class CodeDataConfiguration : IEntityTypeConfiguration<Code_Data>
    {
        public void Configure(EntityTypeBuilder<Code_Data> builder)
        {
            builder.ToTable("Code_Data");
            builder.HasKey(x => x.Data_Id);
            builder.Property(x => x.Data_Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Table_Code).HasMaxLength(50);
            builder.Property(x => x.Data_Code).HasMaxLength(50);
            builder.Property(x => x.Data_Value).HasMaxLength(250);
            builder.Property(x => x.ModifiedBy).HasMaxLength(50);
            builder.Property(x => x.CreatedBy).HasMaxLength(50);
            builder.Property(x => x.DeletedBy).HasMaxLength(50);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
