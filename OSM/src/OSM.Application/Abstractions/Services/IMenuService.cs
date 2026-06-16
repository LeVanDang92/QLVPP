using OSM.Application.Features.BaseSetup.MenuSetup;
using OSM.Application.Features.BaseSetup.MenuSetup.CreateMenu;
using OSM.Application.Features.BaseSetup.MenuSetup.UpdateMenu;

namespace OSM.Application.Abstractions.Services
{
    public interface IMenuService
    {
        Task<MenuResponse> UpdateMenu(UpdateMenuCommand command, CancellationToken cancellationToken);
        Task<MenuResponse> CreateMenu(CreateMenuCommand command, CancellationToken cancellationToken);
        Task<bool> DeleteMenu(string MenuId, CancellationToken cancellationToken);
    }
}
