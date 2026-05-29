using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Features.Auth.Login;
using OSM.Application.Features.Auth.RefreshToken;
using OSM.Application.Features.Auth.Register;
using OSM.Application.Features.Auth.RevokeRefreshToken;
using OSM.Infrastructure.Common;

namespace OSM.API.Controllers
{
    [ApiVersion("1.0")]
    public sealed class AuthController(ISender sender, ICurrentUserService currentUserService, IIdentityService identityService) : ApiAuthBaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            // Attempt to retrieve the refresh token from the cookies
            var refreshToken = Request.Cookies[Constants.REFRESH_TOKEN];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Refresh token is missing."
                });
            }

            command = command with { RefreshToken = refreshToken };

            var result = await sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(currentUserService.UserId))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "User is not authenticated."
                });
            }

            var currentUser = await identityService.GetCurrentUserAsync(currentUserService.UserId, cancellationToken);
            if (currentUser is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "User does not exist."
                });
            }

            return Ok(currentUser);
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[Constants.REFRESH_TOKEN];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var command = new RevokeRefreshTokenCommand(refreshToken);
                var result = await sender.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    // Clear the refresh token cookie
                    Response.Cookies.Delete(Constants.REFRESH_TOKEN);
                }

                return HandleResult(result);
            }

            return NotFound(new { Message = "Refresh token not found." });
        }
    }
}
