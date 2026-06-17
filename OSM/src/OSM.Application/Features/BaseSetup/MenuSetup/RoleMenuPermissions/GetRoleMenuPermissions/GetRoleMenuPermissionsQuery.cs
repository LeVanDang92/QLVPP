using OSM.Application.Abstractions.Messaging;
using OSM.Application.Features.BaseSetup.RoleMenuPermissions;

namespace OSM.Application.Features.BaseSetup.RoleMenuPermissions.GetRoleMenuPermissions;

public sealed record GetRoleMenuPermissionsQuery(Guid RoleId) : IQuery<List<RoleMenuPermissionResponse>>;
