using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.Auth.Register
{
    public sealed class RegisterCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterCommand, Result<Guid>>
    {
        public Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
            => identityService.RegisterAsync(request.UserName, request.Email, request.Password, cancellationToken);
    }

}
