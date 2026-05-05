using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Auth.Login
{
    public sealed record LoginCommand(string UserNameOrEmail, string Password) : ICommand<TokenResponse>;
}
