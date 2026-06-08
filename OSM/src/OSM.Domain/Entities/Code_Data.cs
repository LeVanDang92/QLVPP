using OSM.Domain.Common;

namespace OSM.Domain.Entities
{
    public sealed class Code_Data : IAuditableEntity, ISoftDelete
    {
        public int Data_Id { get; set; }
        public string Table_Code { get; set; }
        public string Data_Code { get; set; }
        public string Data_Value { get; set; }
        public int Sort_Order { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
