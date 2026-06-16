using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.MenuSetup.UpdateMenu
{
    public sealed record UpdateMenuCommand(
                               string MenuId,
                               string MenuName,
                               string MenuShortName,
                               string MenuType,
                               string MenuGroup,
                               string? MenuUrl,
                               string? ExternalUrl,
                               string IconClass,
                               int DisplayOrder,
                               string? BadgeText,
                               string? BadgeClass,
                               string? ParentMenuId,
                               bool Closable,
                               bool IsActive) : ICommand<MenuResponse>;
}