using MediatR;
using OSM.Application.Abstractions.Services;
using OSM.Application.Common;
using OSM.Application.Common.Errors;

namespace OSM.Application.Features.BaseSetup.MenuSetup.DeleteMenu
{
    public sealed class DeleteMenuCommandHandler(IMenuService menuService) : IRequestHandler<DeleteMenuCommand, Result>
    {
        public async Task<Result> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
           bool result = await menuService.DeleteMenu(request.MenuId, cancellationToken);
            return result ? Result.Success() : Result.Failure(Error.Unexpected("Menu.DeleteFailed","Failed to delete menu."));
        }
    }
}
