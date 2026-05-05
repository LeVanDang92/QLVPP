namespace OSM.Infrastructure.Audit
{
    public sealed class AuditLog
    {
        public long Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? KeyValues { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? ChangedColumns { get; set; }
        public string? UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
