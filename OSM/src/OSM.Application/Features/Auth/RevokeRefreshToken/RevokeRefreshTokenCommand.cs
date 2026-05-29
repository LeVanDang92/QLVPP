using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Auth.RevokeRefreshToken
{
    public sealed record RevokeRefreshTokenCommand(string RefreshToken) : ICommand<bool>;
}
