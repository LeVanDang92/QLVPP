using MediatR;
using Microsoft.Extensions.Logging;
using OSM.Application.Common.DomainEvents;
using OSM.Application.Common.Emails;
using OSM.Domain.Entities.Products;

namespace OSM.Application.Features.Products.Events;

public sealed class ProductCreatedDomainEventHandler
    : INotificationHandler<DomainEventNotification<ProductCreatedDomainEvent>>
{
    private readonly ILogger<ProductCreatedDomainEventHandler> _logger;
    private readonly IEmailSender _mailSender;

    public ProductCreatedDomainEventHandler(
        ILogger<ProductCreatedDomainEventHandler> logger , IEmailSender email)
    {
        _logger = logger;
        _mailSender = email;
    }

    public Task Handle(
        DomainEventNotification<ProductCreatedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Product created domain event handled. ProductId: {ProductId}, OccurredOn: {OccurredOn}",
            domainEvent.ProductId,
            domainEvent.OccurredOn);

        _mailSender.SendEmail(new Message(["h2105001@wisol.co.kr"], "product created", "product create"));

        return Task.CompletedTask;
    }
}
