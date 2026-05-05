using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM.Domain.Common
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeletedAt { get; set; }
        string? DeletedBy { get; set; }
    }
}
