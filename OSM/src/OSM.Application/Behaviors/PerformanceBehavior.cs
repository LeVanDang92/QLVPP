using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace OSM.Application.Behaviors
{
    public sealed class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 5000)
            {
                logger.LogWarning("Long running request {RequestName}: {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
            }

            return response;
        }
    }
}
