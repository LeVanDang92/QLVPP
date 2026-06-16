using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.RoleSetup.CreateRole
{
    public sealed record CreateRoleCommand(string Name, string Description) : ICommand<RoleResponse>;
}
