using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.UserSetup.UpdateUser
{
    public sealed record UpdateUserCommand(string UserName, string FullName,string Password,string Email,bool IsActive,string Department,string Role) : ICommand<UserResponse>;
}
