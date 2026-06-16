using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.RoleSetup.UpdateRole
{
    public sealed record UpdateRoleCommand(Guid Id, string Name, string Description) : ICommand<RoleResponse>;   
}
