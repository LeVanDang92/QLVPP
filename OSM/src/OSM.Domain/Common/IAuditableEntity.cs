namespace OSM.Domain.Common
{
    public interface IAuditableEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTimeOffset? ModifiedAt { get; set; }
        string? ModifiedBy { get; set; }
    }
}
