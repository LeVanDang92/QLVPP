using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Features.BaseSetup.RoleSetup.CreateRole;
using OSM.Application.Features.BaseSetup.RoleSetup.DeleteRole;
using OSM.Application.Features.BaseSetup.RoleSetup.GetRoles;
using OSM.Application.Features.BaseSetup.RoleSetup.UpdateRole;

namespace OSM.API.Controllers.BaseSetup
{
    [Authorize]
    public class RoleSettingController(ISender sender) : ApiController
    {
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
           var result = await sender.Send(new GetRoleQuery(), cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole(CreateRoleCommand command, CancellationToken cancellationToken)
        {
           var result = await sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        [HttpPut("roles/{Id:Guid}")]
        public async Task<IActionResult> UpdateRole(Guid Id,UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("roles/{Id:guid}")]
        public async Task<IActionResult> DeleteRole(Guid Id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteRoleCommand(Id), cancellationToken);
            return HandleResult(result);
        }
    }
}
