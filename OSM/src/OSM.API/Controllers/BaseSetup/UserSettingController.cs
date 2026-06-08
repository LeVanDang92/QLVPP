using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Features.BaseSetup.UserSetup.GetUsers;

namespace OSM.API.Controllers.BaseSetup
{
    [ApiVersion("1.0")]
    [Authorize]
    public class UserSettingController(ISender sender) : ApiController
    {
        [HttpGet("users")]
        public async Task<IActionResult> GetUserSettings(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetUsersQuery(), cancellationToken);
            return HandleResult(result);
        }
    }
}
