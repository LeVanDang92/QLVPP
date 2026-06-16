using MediatR;
using OSM.Application.Abstractions.Services;
using OSM.Application.Common;
using OSM.Application.Common.Errors;

namespace OSM.Application.Features.BaseSetup.MenuSetup.UpdateMenu
{
    public sealed class UpdateMenuCommandHandler(IMenuService menuService) : IRequestHandler<UpdateMenuCommand, Result<MenuResponse>>
    {
        public async Task<Result<MenuResponse>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
           var menu = await menuService.UpdateMenu(request,cancellationToken);

            if (menu is null)
            {
                return Result.Failure<MenuResponse>(
                    Error.NotFound("Menu.NotFound", "Menu does not exist.")
                );
            }

            return Result.Success(menu);
        }
    }
}
