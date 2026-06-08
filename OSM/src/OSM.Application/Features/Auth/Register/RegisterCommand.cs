using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Auth.Register
{
    public sealed record RegisterCommand(string FullName,string UserName, string Email, string PasswordShow, string Role,string Department,bool IsActive) : ICommand<Guid>;
}
