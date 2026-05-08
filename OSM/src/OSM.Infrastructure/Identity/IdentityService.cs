using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;
using OSM.Application.Common.Errors;
using OSM.Application.Features.Auth;
using OSM.Infrastructure.Authentication;
using OSM.Infrastructure.Common;
using OSM.Infrastructure.Persistence;

namespace OSM.Infrastructure.Identity
{
    public sealed class IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions,
        IOptions<TokenHashingOptions> tokenHashingOptions) : IIdentityService
    {
        public async Task<Result<Guid>> RegisterAsync(string userName, string email, string password, CancellationToken cancellationToken)
        {
            if (await userManager.FindByNameAsync(userName) is not null)
            {
                return Result.Failure<Guid>(Error.Conflict("Identity.UserNameDuplicated", "Username already exists."));
            }

            if (await userManager.FindByEmailAsync(email) is not null)
            {
                return Result.Failure<Guid>(Error.Conflict("Identity.EmailDuplicated", "Email already exists."));
            }

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Result.Failure<Guid>(ToValidationError(result.Errors, nameof(password)));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = "User" });
                if (!roleResult.Succeeded)
                {
                    return Result.Failure<Guid>(ToValidationError(roleResult.Errors, nameof(ApplicationRole)));
                }
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, "User");
            if (!addRoleResult.Succeeded)
            {
                return Result.Failure<Guid>(ToValidationError(addRoleResult.Errors, nameof(ApplicationRole)));
            }

            return Result.Success(user.Id);
        }

        public async Task<Result<TokenResponse>> LoginAsync(string userNameOrEmail, string password, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(userNameOrEmail)
                ?? await userManager.FindByEmailAsync(userNameOrEmail);

            if (user is null)
            {
                return Result.Failure<TokenResponse>(Error.Unauthorized("Identity.InvalidCredentials", "Invalid username or password."));
            }

            if (await userManager.IsLockedOutAsync(user))
            {
                return Result.Failure<TokenResponse>(Error.Unauthorized("Identity.LockedOut", "User account is temporarily locked."));
            }

            if (!await userManager.CheckPasswordAsync(user, password))
            {
                await userManager.AccessFailedAsync(user);
                return Result.Failure<TokenResponse>(Error.Unauthorized("Identity.InvalidCredentials", "Invalid username or password."));
            }

            await userManager.ResetAccessFailedCountAsync(user);

            return await CreateTokenResponseAsync(user, cancellationToken);
        }

        public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var hashedToken = TokenHasher.HashToken(refreshToken, tokenHashingOptions.Value.Pepper);

            var token = await dbContext.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == hashedToken, cancellationToken);

            if (token is null || !token.IsActive)
            {
                return Result.Failure<TokenResponse>(Error.Unauthorized("Identity.InvalidRefreshToken", "Invalid refresh token."));
            }

            token.RevokedAt = DateTimeOffset.UtcNow;
            return await CreateTokenResponseAsync(token.User, cancellationToken);
        }

        public async Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var id))
            {
                return null;
            }

            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user is null)
            {
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);
            var roleIds = await GetRoleIdsAsync(roles, cancellationToken);
            var menus = await GetMenuPermissionsAsync(roleIds, cancellationToken);
            var permissions = menus
                .SelectMany(x => x.PermissionKeys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToArray();

            return new CurrentUserResponse(
                user.Id.ToString(),
                user.UserName ?? string.Empty,
                roles.ToArray(),
                permissions,
                menus);
        }

        private async Task<Result<TokenResponse>> CreateTokenResponseAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roles = await userManager.GetRolesAsync(user);
            var roleIds = await GetRoleIdsAsync(roles, cancellationToken);
            var menus = await GetMenuPermissionsAsync(roleIds, cancellationToken);
            var permissions = menus
                .SelectMany(x => x.PermissionKeys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var accessToken = jwtTokenService.CreateAccessToken(
                user.Id.ToString(),
                user.UserName ?? user.Email ?? string.Empty,
                roles,
                permissions);

            var refreshToken = jwtTokenService.CreateRefreshToken();
            var expiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays);

            dbContext.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = TokenHasher.HashToken(refreshToken, tokenHashingOptions.Value.Pepper),
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = expiresAt
            });

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(new TokenResponse(
                accessToken,
                refreshToken,
                DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes)));
        }

        private async Task<List<Guid>> GetRoleIdsAsync(IEnumerable<string> roleNames, CancellationToken cancellationToken)
        {
            var roleNameArray = roleNames.ToArray();

            return await roleManager.Roles
                .AsNoTracking()
                .Where(r => r.Name != null && roleNameArray.Contains(r.Name))
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);
        }

        private async Task<IReadOnlyCollection<MenuPermissionResponse>> GetMenuPermissionsAsync(
            IReadOnlyCollection<Guid> roleIds,
            CancellationToken cancellationToken)
        {
            if (roleIds.Count == 0)
            {
                return [];
            }

            var rows = await dbContext.RoleMenuPermissions
                .AsNoTracking()
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => new
                {
                    rp.Menu.MenuId,
                    rp.Menu.MenuName,
                    rp.Menu.MenuShortName,
                    rp.Menu.MenuType,
                    rp.Menu.MenuGroup,
                    rp.Menu.MenuUrl,
                    rp.Menu.IconIndex,
                    rp.PermissionId
                })
                .ToListAsync(cancellationToken);

            return rows
                .GroupBy(x => new
                {
                    x.MenuId,
                    x.MenuName,
                    x.MenuShortName,
                    x.MenuType,
                    x.MenuGroup,
                    x.MenuUrl,
                    x.IconIndex
                })
                .Select(group => new MenuPermissionResponse(
                    group.Key.MenuId,
                    group.Key.MenuName,
                    group.Key.MenuShortName,
                    group.Key.MenuType,
                    group.Key.MenuGroup,
                    group.Key.MenuUrl,
                    group.Key.IconIndex,
                    group.Select(x => x.PermissionId)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToArray(),
                    group.Select(x => $"{x.MenuId}.{x.PermissionId}")
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(x => x)
                        .ToArray()))
                .OrderBy(x => x.MenuGroup)
                .ThenBy(x => x.MenuName)
                .ToArray();
        }

        private static Error ToValidationError(IEnumerable<IdentityError> identityErrors, string propertyName)
        {
            return Error.Validation(identityErrors
                .Select(x => new ValidationError(propertyName, x.Description))
                .ToArray());
        }
    }
}
