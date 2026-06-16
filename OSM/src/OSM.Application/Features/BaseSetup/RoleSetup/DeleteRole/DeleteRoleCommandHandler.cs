using MediatR;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.RoleSetup.DeleteRole
{
    public sealed class DeleteRoleCommandHandler(IIdentityService identityService) : IRequestHandler<DeleteRoleCommand, Result>
    {
        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
           bool isDeleted = await identityService.DeleteRoleAsync(request.Id, cancellationToken);
           return isDeleted ? Result.Success(isDeleted) : Result.Failure(new Common.Errors.Error("","Delete fault",Common.Errors.ErrorType.None));
        }
    }
}
