using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OSM.Domain.Exceptions;
using OSM.Infrastructure.Storage.Synology;
using System.Net;

namespace OSM.API.Middleware
{
    public sealed class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problem = exception switch
            {
                ValidationException validationException => CreateValidationProblem(context, validationException),
                NotFoundException notFoundException => CreateProblem(context, HttpStatusCode.NotFound, "Not Found", notFoundException.Message),
                DomainException domainException => CreateProblem(context, HttpStatusCode.BadRequest, "Bad Request", domainException.Message),
                UnauthorizedAccessException unauthorizedAccessException => CreateProblem(context, HttpStatusCode.Forbidden, "Forbidden", unauthorizedAccessException.Message),
                SynologyException => CreateProblem(context, HttpStatusCode.BadGateway, "Storage service error", "Storage service is unavailable or returned an error."),
                _ => CreateProblem(context, HttpStatusCode.InternalServerError, "Server error", "An unexpected error occurred.")
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }

        private static ProblemDetails CreateValidationProblem(HttpContext context, ValidationException validationException)
        {
            return new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request",
                Detail = "One or more validation errors occurred.",
                Type = "https://httpstatuses.com/400",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["errorCode"] = "Validation.Error",
                    ["errors"] = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(e => e.ErrorMessage).ToArray())
                }
            };
        }

        private static ProblemDetails CreateProblem(
            HttpContext context,
            HttpStatusCode statusCode,
            string title,
            string detail)
        {
            return new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Type = $"https://httpstatuses.com/{(int)statusCode}",
                Instance = context.Request.Path
            };
        }
    }
}
