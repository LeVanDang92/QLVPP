using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Features.BaseSetup.RoleSetup.GetRoles;

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
    }
}
