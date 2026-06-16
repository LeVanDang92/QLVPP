using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Common;
using OSM.Application.Common.Errors;
using OSM.Application.Features.BaseSetup.MenuSetup;
using OSM.Application.Features.BaseSetup.MenuSetup.CreateMenu;
using OSM.Application.Features.BaseSetup.MenuSetup.DeleteMenu;
using OSM.Application.Features.BaseSetup.MenuSetup.GetMenu;
using OSM.Application.Features.BaseSetup.MenuSetup.UpdateMenu;

namespace OSM.API.Controllers.BaseSetup
{
    [Authorize]
    public class MenusController(ISender sender) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetMenu(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetMenuQuery(), cancellationToken);

            return HandleResult(result);
        }

        [HttpPut("{MenuId}")]
        [ProducesResponseType(typeof(MenuResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMenu(string MenuId,UpdateMenuCommand request, CancellationToken cancellationToken)
        {

            if (MenuId != request.MenuId)
            {
                return HandleResult(Result.Failure(
                    Error.Validation([
                        new ValidationError("menuId", "Route menuId does not match body menuId.")
                    ])
                ));
            }

            var result = await sender.Send(request, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MenuResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMenu(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{MenuId}")]
        [ProducesResponseType(typeof(MenuResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMenu(string MenuId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteMenuCommand(MenuId), cancellationToken);
            return HandleResult(result);
        }
    }
}
