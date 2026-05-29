using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.Auth.RevokeRefreshToken
{
    /// <summary>
    /// Handles the command to revoke a refresh token.
    /// </summary>
    public sealed class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevokeRefreshTokenCommandHandler"/> class.
        /// </summary>
        /// <param name="identityService">The identity service.</param>
        public RevokeRefreshTokenCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <inheritdoc />
        public async Task<Result<bool>> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            bool revoke = await _identityService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Result.Success(revoke);
        }
    }
}
