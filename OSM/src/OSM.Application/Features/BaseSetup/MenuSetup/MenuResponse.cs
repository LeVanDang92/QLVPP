namespace OSM.Application.Features.BaseSetup.MenuSetup
{
    /// <param name="MenuId"></param>
    /// <param name="MenuName"></param>
    /// <param name="MenuShortName"></param>
    /// <param name="MenuType"></param>
    /// <param name="MenuGroup"></param>
    /// <param name="MenuUrl"></param>
    /// <param name="ExternalUrl"> Dùng cho link bên ngoài app Angular. </param>
    /// <param name="IconClass"></param>
    /// <param name="DisplayOrder"></param>
    /// <param name="BadgeText"> Dùng để gắn nhãn cho menu </param>
    /// <param name="BadgeClass"></param>
    /// <param name="ParentMenuId"> Menu cha </param>
    public record MenuResponse(string MenuId,
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
                               bool IsActive);
}
