using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Features.BaseSetup.UserSetup.DeleteUser;
using OSM.Application.Features.BaseSetup.UserSetup.GetUsers;
using OSM.Application.Features.BaseSetup.UserSetup.UpdateUser;

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

        [HttpPut("users/{userName}")]
        public async Task<IActionResult> UpdateUser(string userName, UpdateUserCommand request,CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("users/{userName}")]
        public async Task<IActionResult> DeleteUser(string userName, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteUserCommand(userName), cancellationToken);
            return HandleResult(result);
        }
    }
}
