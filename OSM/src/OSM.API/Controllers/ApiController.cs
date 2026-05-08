using Microsoft.AspNetCore.Mvc;
using OSM.Application.Common;
using OSM.Application.Common.Errors;

namespace OSM.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok();
            }

            return HandleFailure(result.Error);
        }

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return HandleFailure(result.Error);
        }

        private IActionResult HandleFailure(Error error)
        {
            var statusCode = GetStatusCode(error.Type);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(error.Type),
                Detail = error.Description,
                Type = GetProblemType(error.Type)
            };

            problemDetails.Instance = HttpContext.Request.Path;
            problemDetails.Extensions["errorCode"] = error.Code;
            problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;

            if (error.Type == ErrorType.Validation && error.ValidationErrors is not null)
            {
                problemDetails.Extensions["errors"] = error.ValidationErrors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(x => x.ErrorMessage).ToArray());
            }

            return StatusCode(statusCode, problemDetails);
        }

        private static int GetStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetTitle(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => "Bad Request",
                ErrorType.Unauthorized => "Unauthorized",
                ErrorType.Forbidden => "Forbidden",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                ErrorType.Unexpected => "Internal Server Error",
                _ => "Internal Server Error"
            };
        }

        private static string GetProblemType(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => "https://httpstatuses.com/400",
                ErrorType.Unauthorized => "https://httpstatuses.com/401",
                ErrorType.Forbidden => "https://httpstatuses.com/403",
                ErrorType.NotFound => "https://httpstatuses.com/404",
                ErrorType.Conflict => "https://httpstatuses.com/409",
                ErrorType.Unexpected => "https://httpstatuses.com/500",
                _ => "https://httpstatuses.com/500"
            };
        }
    }
}
