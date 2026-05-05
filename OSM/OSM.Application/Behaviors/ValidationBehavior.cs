using FluentValidation;
using MediatR;

namespace OSM.Application.Behaviors
{
    /// <summary>
    /// Trước khi vào handler, tự động chạy FluentValidation.Nếu invalid thì trả lỗi ngay.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="validators"></param>
    public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

            if (failures.Count == 0) return await next();

            var message = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
            throw new ValidationException(message, failures);
        }
    }
}
