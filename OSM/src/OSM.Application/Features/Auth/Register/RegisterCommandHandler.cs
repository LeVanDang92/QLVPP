using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.Auth.Register
{
    public sealed class RegisterCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
            => await identityService.RegisterAsync(request.FullName,request.UserName, request.Email, request.PasswordShow,request.Role,request.Department,request.IsActive, cancellationToken);
    }

}
