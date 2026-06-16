using MediatR;
using OSM.Application.Abstractions.Services;
using OSM.Application.Common;
using OSM.Application.Common.Errors;

namespace OSM.Application.Features.BaseSetup.UserSetup.UpdateUser
{
    public sealed class UpdateUserCommandHandler(IUserService userService) : IRequestHandler<UpdateUserCommand, Result<UserResponse>>
    {
        public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            UpdateUserRequest userRequest = new UpdateUserRequest(request.UserName, request.FullName, request.Password, request.Email, request.IsActive, request.Department, request.Role);

            var user = await userService.UpdateUserAsync(userRequest);

            return user != null
                ? Result.Success(user)
                : Result.Failure<UserResponse>(Error.Unexpected("User.UpdateFailed", "Failed to update user."));     
        }
    }
}
