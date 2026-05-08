using OSM.Application.Common;
using OSM.Application.Features.Auth;

namespace OSM.Application.Abstractions.Identity
{
    public interface IIdentityService
    {
        Task<Result<Guid>> RegisterAsync(string userName, string email, string password, CancellationToken cancellationToken);
        Task<Result<TokenResponse>> LoginAsync(string userNameOrEmail, string password, CancellationToken cancellationToken);
        Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken);
    }
}
