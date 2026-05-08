using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.Auth.RefreshToken
{
    public sealed class RefreshTokenCommandHandler(IIdentityService identityService) : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
    {
        public Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            => identityService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}
