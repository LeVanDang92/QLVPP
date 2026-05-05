using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSM.Infrastructure.Audit;

namespace OSM.Infrastructure.Persistence.Configurations
{
    public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TableName).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Action).HasMaxLength(50).IsRequired();
            builder.Property(x => x.UserId).HasMaxLength(100);
        }
    }
}
