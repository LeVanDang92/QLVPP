using MediatR;
using Microsoft.EntityFrameworkCore;
using OSM.Application.Abstractions.Data;
using OSM.Application.Abstractions.Messaging;
using OSM.Application.Common;

namespace OSM.Application.Behaviors
{
    /// <summary>
    /// Command chạy xong thì tự SaveChanges.
    /// Query thì không SaveChanges.
    /// Nếu command trả về Result failure thì rollback transaction.
    /// </summary>
    public sealed class TransactionBehavior<TRequest, TResponse>(IApplicationDbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!IsCommand(request))
            {
                return await next();
            }

            var strategy = dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var response = await next();

                if (IsFailureResult(response))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return response;
                }

                var domainEvents = dbContext.GetDomainEvents();

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                dbContext.ClearDomainEvents();

                // public events
                await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

                return response;
            });
        }

        private static bool IsCommand(TRequest request)
        {
            return request is ICommand
                || request.GetType().GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
        }

        private static bool IsFailureResult(TResponse response)
        {
            return response is Result { IsFailure: true };
        }
    }
}
