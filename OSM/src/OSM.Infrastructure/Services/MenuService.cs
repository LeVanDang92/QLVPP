using OSM.Application.Abstractions.Services;
using OSM.Application.Features.BaseSetup.MenuSetup;
using OSM.Application.Features.BaseSetup.MenuSetup.CreateMenu;
using OSM.Application.Features.BaseSetup.MenuSetup.UpdateMenu;
using OSM.Infrastructure.Identity;
using OSM.Infrastructure.Persistence;

namespace OSM.Infrastructure.Services
{
    public class MenuService(ApplicationDbContext _context) : IMenuService
    {
        public Task<MenuResponse> UpdateMenu(UpdateMenuCommand command, CancellationToken cancellationToken)
        {
            var menu = _context.Menus.FirstOrDefault(m => m.MenuId == command.MenuId);

            if (menu != null)
            {
                menu.MenuName = command.MenuName;
                menu.MenuShortName = command.MenuShortName;
                menu.MenuType = command.MenuType;
                menu.MenuGroup = command.MenuGroup;
                menu.MenuUrl = command.MenuUrl;
                menu.ExternalUrl = command.ExternalUrl;
                menu.IconClass = command.IconClass;
                menu.DisplayOrder = command.DisplayOrder;
                menu.BadgeText = command.BadgeText;
                menu.BadgeClass = command.BadgeClass;
                menu.ParentMenuId = command.ParentMenuId;
                menu.Closable = command.Closable;
                menu.IsActive = command.IsActive;

                _context.Menus.Update(menu);

                return Task.FromResult(new MenuResponse
                (
                menu.MenuId,
                menu.MenuName,
                menu.MenuShortName,
                menu.MenuType,
                menu.MenuGroup,
                menu.MenuUrl,
                menu.ExternalUrl,
                menu.IconClass,
                menu.DisplayOrder,
                menu.BadgeText,
                menu.BadgeClass,
                menu.ParentMenuId,
                menu.Closable,
                menu.IsActive
                ));
            }
            return null;
        }

        public async Task<MenuResponse> CreateMenu(CreateMenuCommand command, CancellationToken cancellationToken)
        {
            var menu = _context.Menus.FirstOrDefault(m => m.MenuId == command.MenuId);

            if (menu == null)
            {
                var newMenu = await _context.Menus.AddAsync(new Menus
                {
                    MenuId = command.MenuId,
                    MenuName = command.MenuName,
                    MenuShortName = command.MenuShortName,
                    MenuType = command.MenuType,
                    MenuGroup = command.MenuGroup,
                    MenuUrl = command.MenuUrl,
                    ExternalUrl = command.ExternalUrl,
                    IconClass = command.IconClass,
                    DisplayOrder = command.DisplayOrder,
                    BadgeText = command.BadgeText,
                    BadgeClass = command.BadgeClass,
                    ParentMenuId = command.ParentMenuId,
                    Closable = command.Closable,
                    IsActive = command.IsActive
                });

                return new MenuResponse
                (
                   newMenu.Entity.MenuId,
                   newMenu.Entity.MenuName,
                   newMenu.Entity.MenuShortName,
                   newMenu.Entity.MenuType,
                   newMenu.Entity.MenuGroup,
                   newMenu.Entity.MenuUrl,
                   newMenu.Entity.ExternalUrl,
                   newMenu.Entity.IconClass,
                   newMenu.Entity.DisplayOrder,
                   newMenu.Entity.BadgeText,
                   newMenu.Entity.BadgeClass,
                   newMenu.Entity.ParentMenuId,
                   newMenu.Entity.Closable,
                   newMenu.Entity.IsActive
                );
            }
            return null;
        }

        public Task<bool> DeleteMenu(string MenuId, CancellationToken cancellationToken)
        {
            _context.Menus.RemoveRange(_context.Menus.Where(m => m.MenuId == MenuId));
            return Task.FromResult(true);
        }
    }
}