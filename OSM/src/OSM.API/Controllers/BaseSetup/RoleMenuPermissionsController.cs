using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Features.BaseSetup.RoleMenuPermissions;
using OSM.Application.Features.BaseSetup.RoleMenuPermissions.GetRoleMenuPermissions;
using OSM.Application.Features.BaseSetup.RoleMenuPermissions.UpdateRoleMenuPermissions;

namespace OSM.API.Controllers.BaseSetup;

[Authorize]
public sealed class RoleMenuPermissionsController(ISender sender) : ApiController
{
    [HttpGet("{roleId:guid}")]
    [ProducesResponseType(typeof(List<RoleMenuPermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByRole(Guid roleId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRoleMenuPermissionsQuery(roleId), cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{roleId:guid}")]
    [ProducesResponseType(typeof(List<RoleMenuPermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateByRole(
        Guid roleId,
        UpdateRoleMenuPermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateRoleMenuPermissionsCommand(roleId, request.Items),
            cancellationToken);

        return HandleResult(result);
    }
}
