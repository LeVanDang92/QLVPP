using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.BaseSetup.MenuSetup.DeleteMenu
{
    public sealed record DeleteMenuCommand(string MenuId) : ICommand;
}
