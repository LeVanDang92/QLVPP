using MediatR;
using OSM.Application.Abstractions.Services;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.MenuSetup.CreateMenu
{
    public sealed class CreateMenuCommandHandler(IMenuService menuService) : IRequestHandler<CreateMenuCommand, Result<MenuResponse>>
    {
        public async Task<Result<MenuResponse>> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var data = await menuService.CreateMenu(request, cancellationToken);
            return Result.Success(data);
        }
    }
}
