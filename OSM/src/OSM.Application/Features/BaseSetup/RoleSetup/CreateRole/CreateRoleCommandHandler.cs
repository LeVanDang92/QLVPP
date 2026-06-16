using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.RoleSetup.CreateRole
{
    public sealed class CreateRoleCommandHandler(IIdentityService identityService) : IRequestHandler<CreateRoleCommand, Result<RoleResponse>>
    {
        public async Task<Result<RoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await identityService.CreateRoleAsync(request, cancellationToken);

            return Result.Success(role);
        }
    }
}
