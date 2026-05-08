using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Auth.RefreshToken
{
    public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<TokenResponse>;
}
