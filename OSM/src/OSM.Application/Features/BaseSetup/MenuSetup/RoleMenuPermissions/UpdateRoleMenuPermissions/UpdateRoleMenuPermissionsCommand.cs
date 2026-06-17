using OSM.Application.Abstractions.Messaging;
using OSM.Application.Features.BaseSetup.RoleMenuPermissions;

namespace OSM.Application.Features.BaseSetup.RoleMenuPermissions.UpdateRoleMenuPermissions;

public sealed record UpdateRoleMenuPermissionsCommand(
    Guid RoleId,
    List<UpdateRoleMenuPermissionItem> Items) : ICommand<List<RoleMenuPermissionResponse>>;
