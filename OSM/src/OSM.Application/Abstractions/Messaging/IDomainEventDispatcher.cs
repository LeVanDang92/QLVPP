using OSM.Domain.Common;

namespace OSM.Application.Abstractions.Messaging;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents,CancellationToken cancellationToken = default);
}