using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.Auth.Login
{
    public sealed class LoginCommandHandler(IIdentityService identityService) : IRequestHandler<LoginCommand, Result<TokenResponse>>
    {
        public Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
            => identityService.LoginAsync(request.UserNameOrEmail, request.Password, cancellationToken);
    }
}
