using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OSM.Application.Abstractions.Authentication;
using OSM.Domain.Common;
using OSM.Infrastructure.Audit;
using System.Text.Json;

namespace OSM.Infrastructure.Persistence.Interceptors
{
    public sealed class AuditSaveChangesInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAudit(DbContext? context)
        {
            if (context is null) return;

            var now = DateTimeOffset.UtcNow;
            var userId = currentUserService.UserId ?? "system";
            var auditLogs = new List<AuditLog>();

            foreach (var entry in context.ChangeTracker.Entries().ToList())
            {
                if (entry.Entity is AuditLog || entry.State is EntityState.Detached or EntityState.Unchanged)
                    continue;

                if (entry.Entity is IAuditableEntity auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedAt = now;
                        auditable.CreatedBy = userId;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditable.ModifiedAt = now;
                        auditable.ModifiedBy = userId;
                    }
                }

                if (entry.Entity is ISoftDelete softDelete && entry.State == EntityState.Modified && softDelete.IsDeleted)
                {
                    softDelete.DeletedAt ??= now;
                    softDelete.DeletedBy ??= userId;
                }

                var audit = CreateAuditLog(entry, userId, now);
                if (audit is not null) auditLogs.Add(audit);
            }

            if (auditLogs.Count > 0)
                context.Set<AuditLog>().AddRange(auditLogs);
        }

        private static AuditLog? CreateAuditLog(EntityEntry entry, string userId, DateTimeOffset now)
        {
            var tableName = entry.Metadata.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName)) return null;

            var keyValues = new Dictionary<string, object?>();
            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();
            var changedColumns = new List<string>();

            foreach (var property in entry.Properties)
            {
                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    keyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        newValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        oldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified when property.IsModified:
                        changedColumns.Add(propertyName);
                        oldValues[propertyName] = property.OriginalValue;
                        newValues[propertyName] = property.CurrentValue;
                        break;
                }
            }

            return new AuditLog
            {
                TableName = tableName,
                Action = entry.State.ToString(),
                UserId = userId,
                CreatedAt = now,
                KeyValues = JsonSerializer.Serialize(keyValues),
                OldValues = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues),
                NewValues = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues),
                ChangedColumns = changedColumns.Count == 0 ? null : JsonSerializer.Serialize(changedColumns)
            };
        }
    }
}
