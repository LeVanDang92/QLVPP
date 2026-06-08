using OSM.Application.Common;
using OSM.Application.Features.Auth;

namespace OSM.Application.Abstractions.Identity
{
    public interface IIdentityService
    {
        Task<Result<Guid>> RegisterAsync(string fullName, string userName, string email, string password,string role,string department,bool isActive, CancellationToken cancellationToken);
        Task<Result<TokenResponse>> LoginAsync(string userNameOrEmail, string password, CancellationToken cancellationToken);
        Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken);
    }
}
