using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OSM.API.Middleware
{
    public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problem = exception switch
            {
                ValidationException validationException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Validation error",
                    Detail = validationException.Message,
                    Extensions = { ["errors"] = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) }
                },
                _ => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Server error",
                    Detail = "An unexpected error occurred."
                }
            };

            context.Response.StatusCode = problem.Status ?? 500;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
