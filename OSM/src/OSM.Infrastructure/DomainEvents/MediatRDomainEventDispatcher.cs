using MediatR;
using OSM.Application.Abstractions.Messaging;
using OSM.Application.Common.DomainEvents;
using OSM.Domain.Common;

namespace OSM.Infrastructure.DomainEvents;

public sealed class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public MediatRDomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents,CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(notificationType, domainEvent);

            if (notification is null)
            {
                continue;
            }

            await _publisher.Publish(notification, cancellationToken);
        }
    }
}