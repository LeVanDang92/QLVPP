using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Behaviors
{
    /// <summary>
    /// Command chạy xong thì tự SaveChanges.
    /// Query thì không SaveChanges.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="dbContext"></param>
    public sealed class TransactionBehavior<TRequest, TResponse>(IApplicationDbContext dbContext)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (IsCommand(request))
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return response;
        }

        private static bool IsCommand(TRequest request)
            => request is ICommand || request.GetType().GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }
}
