using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.MenuSetup.GetMenu
{
    public sealed record GetMenuQuery : IQuery<List<MenuResponse>>;
}
