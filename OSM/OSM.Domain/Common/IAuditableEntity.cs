using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
