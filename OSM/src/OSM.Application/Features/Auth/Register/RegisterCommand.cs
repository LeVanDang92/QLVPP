using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Auth.Register
{
    public sealed record RegisterCommand(string FullName,string UserName, string Email, string Password,string Role) : ICommand<Guid>;
}
