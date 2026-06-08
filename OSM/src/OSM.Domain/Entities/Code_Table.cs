using OSM.Domain.Common;

namespace OSM.Domain.Entities
{
    public sealed class Code_Table : IAuditableEntity, ISoftDelete
    {
        public string Table_Code { get; set; }
        public string Table_Name { get; set; }
        public string Table_Group { get; set; }
        public string Description { get; set; }
        public bool Is_System { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
