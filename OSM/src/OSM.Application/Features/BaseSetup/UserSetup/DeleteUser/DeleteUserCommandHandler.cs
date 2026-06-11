using MediatR;
using OSM.Application.Abstractions.Services;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.UserSetup.DeleteUser
{
    public sealed class DeleteUserCommandHandler(IUserService userService) : IRequestHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.DeleteUserAsync(request.UserName);
            return result ? Result.Success(result) : Result.Failure(new Common.Errors.Error("","Delete user failed.",Common.Errors.ErrorType.None));
        }
    }
}
