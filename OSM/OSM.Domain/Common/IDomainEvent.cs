using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM.Domain.Common
{
    public interface IDomainEvent
    {
        DateTimeOffset OccurredOn { get; }
    }
}
