using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;
using OSM.Application.Features.Auth;
using OSM.Infrastructure.Authentication;
using OSM.Infrastructure.Persistence;

namespace OSM.Infrastructure.Identity
{
    public sealed class IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions) : IIdentityService
    {
        public async Task<Result<Guid>> RegisterAsync(string userName, string email, string password, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser { UserName = userName, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var message = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result.Failure<Guid>(new Error("Identity.RegisterFailed", message));
            }

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new ApplicationRole { Name = "User" });

            await userManager.AddToRoleAsync(user, "User");
            return Result.Success(user.Id);
        }

        public async Task<Result<TokenResponse>> LoginAsync(string userNameOrEmail, string password, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(userNameOrEmail)
                ?? await userManager.FindByEmailAsync(userNameOrEmail);

            if (user is null || !await userManager.CheckPasswordAsync(user, password))
                return Result.Failure<TokenResponse>(new Error("Identity.InvalidCredentials", "Invalid username or password."));

            return await CreateTokenResponseAsync(user, cancellationToken);
        }

        public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var token = await dbContext.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

            if (token is null || !token.IsActive)
                return Result.Failure<TokenResponse>(new Error("Identity.InvalidRefreshToken", "Invalid refresh token."));

            token.RevokedAt = DateTimeOffset.UtcNow;
            return await CreateTokenResponseAsync(token.User, cancellationToken);
        }

        public async Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var id)) return null;
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user is null) return null;

            var roles = await userManager.GetRolesAsync(user);
            var permissions = await GetPermissionsAsync(user, cancellationToken);
            return new CurrentUserResponse(user.Id.ToString(), user.UserName ?? string.Empty, roles.ToArray(), permissions.ToArray());
        }

        private async Task<Result<TokenResponse>> CreateTokenResponseAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roles = await userManager.GetRolesAsync(user);
            var permissions = await GetPermissionsAsync(user, cancellationToken);
            var accessToken = jwtTokenService.CreateAccessToken(user.Id.ToString(), user.UserName ?? user.Email ?? string.Empty, roles, permissions);
            var refreshToken = jwtTokenService.CreateRefreshToken();
            var expiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays);

            dbContext.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = expiresAt
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success(new TokenResponse(accessToken, refreshToken, DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes)));
        }

        private async Task<IReadOnlyCollection<string>> GetPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roleNames = await userManager.GetRolesAsync(user);
            var roleIds = await roleManager.Roles
                .Where(r => roleNames.Contains(r.Name!))
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            return await dbContext.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
