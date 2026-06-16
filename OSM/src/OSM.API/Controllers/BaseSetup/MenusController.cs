using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> UpdateMenu(string MenuId,UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{MenuId}")]
        public async Task<IActionResult> DeleteMenu(string MenuId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteMenuCommand(MenuId), cancellationToken);
            return HandleResult(result);
        }
    }
}
