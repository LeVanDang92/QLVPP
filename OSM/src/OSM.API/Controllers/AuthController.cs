using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Features.Auth.Login;
using OSM.Application.Features.Auth.RefreshToken;
using OSM.Application.Features.Auth.Register;

namespace OSM.API.Controllers
{
    [ApiVersion("1.0")]
    public sealed class AuthController(ISender sender, ICurrentUserService currentUserService, IIdentityService identityService) : ApiController
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
    }
}
