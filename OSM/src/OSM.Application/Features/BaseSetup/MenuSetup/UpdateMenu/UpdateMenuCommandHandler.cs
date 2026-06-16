using MediatR;
using OSM.Application.Abstractions.Services;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.MenuSetup.UpdateMenu
{
    public sealed class UpdateMenuCommandHandler(IMenuService menuService) : IRequestHandler<UpdateMenuCommand, Result<MenuResponse>>
    {
        public async Task<Result<MenuResponse>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
           var menu = await menuService.UpdateMenu(request,cancellationToken);
           return Result.Success(menu);
        }
    }
}
