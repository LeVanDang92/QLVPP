using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.RoleSetup.DeleteRole
{
    public sealed record DeleteRoleCommand(Guid Id) : ICommand;
}
