using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.RoleSetup.GetRoles
{
    public record GetRoleQuery : IQuery<List<RoleResponse>>
    {
    }
}
