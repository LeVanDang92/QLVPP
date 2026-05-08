using FluentValidation;
using MediatR;
using OSM.Application.Common;
using OSM.Application.Common.Errors;

namespace OSM.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(validator =>
                validator.ValidateAsync(context, cancellationToken)));

        var validationErrors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .Select(error => new ValidationError(
                error.PropertyName,
                error.ErrorMessage))
            .Distinct()
            .ToArray();

        if (validationErrors.Length == 0)
        {
            return await next();
        }

        var error = Error.Validation(validationErrors);

        return CreateValidationResult<TResponse>(error);
    }

    private static TResponse CreateValidationResult<TResult>(Error error)
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResult).IsGenericType &&
            typeof(TResult).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResult).GetGenericArguments()[0];

            var resultType = typeof(Result<>).MakeGenericType(valueType);

            var failureMethod = resultType.GetMethod(
                nameof(Result<object>.Failure),
                [typeof(Error)]);

            if (failureMethod is null)
            {
                throw new InvalidOperationException("Could not find Result<T>.Failure method.");
            }

            var result = failureMethod.Invoke(null, [error]);

            return (TResponse)result!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior cannot create validation result for type {typeof(TResult).Name}.");
    }
}