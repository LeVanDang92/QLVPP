using OSM.Domain.Common;

namespace OSM.Domain.Entities.Products
{
    public sealed record ProductCreatedDomainEvent(Guid ProductId) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }

}
