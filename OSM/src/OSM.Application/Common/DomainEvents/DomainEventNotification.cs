using MediatR;
using OSM.Domain.Common;

namespace OSM.Application.Common.DomainEvents;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification where TDomainEvent : IDomainEvent;