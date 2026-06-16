using OSM.Application.Common;
using OSM.Application.Features.Auth;
using OSM.Application.Features.BaseSetup.RoleSetup;
using OSM.Application.Features.BaseSetup.RoleSetup.CreateRole;
using OSM.Application.Features.BaseSetup.RoleSetup.UpdateRole;

namespace OSM.Application.Abstractions.Identity
{
    public interface IIdentityService
    {
        Task<Result<Guid>> RegisterAsync(string fullName, string userName, string email, string password,string role,string department,bool isActive, CancellationToken cancellationToken);
        Task<Result<TokenResponse>> LoginAsync(string userNameOrEmail, string password, CancellationToken cancellationToken);
        Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken);

        Task<RoleResponse> CreateRoleAsync(CreateRoleCommand command, CancellationToken cancellationToken);
        Task<RoleResponse> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
        Task<bool> DeleteRoleAsync(Guid Id, CancellationToken cancellationToken);
    }
}
