using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.RoleSetup.UpdateRole
{
    public sealed class UpdateRoleCommandHandler(IIdentityService identityService) : IRequestHandler<UpdateRoleCommand, Result<RoleResponse>>
    {
        public async Task<Result<RoleResponse>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await identityService.UpdateRoleAsync(request, cancellationToken);

            return Result.Success(role);
        }
    }
}
