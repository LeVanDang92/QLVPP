using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Common;
using OSM.Application.Common.Errors;
using OSM.Application.Features.Auth;
using OSM.Application.Features.BaseSetup.RoleSetup;
using OSM.Application.Features.BaseSetup.RoleSetup.CreateRole;
using OSM.Application.Features.BaseSetup.RoleSetup.UpdateRole;
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
        IOptions<TokenHashingOptions> tokenHashingOptions, IHttpContextAccessor httpContextAccessor) : IIdentityService
    {
        public async Task<Result<Guid>> RegisterAsync(string fullName, string userName, string email, string password, string role, string department, bool isActive, CancellationToken cancellationToken)
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
                FullName = fullName,
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                PasswordShow = password,
                Department = department,
                IsActive = isActive,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System",
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Result.Failure<Guid>(ToValidationError(result.Errors, nameof(password)));
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = role });
                if (!roleResult.Succeeded)
                {
                    return Result.Failure<Guid>(ToValidationError(roleResult.Errors, nameof(ApplicationRole)));
                }
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, role);
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

            if (!user.IsActive)
            {
                return Result.Failure<TokenResponse>(Error.Unauthorized("The account is inactive", "The account is inactive. Please contact the administrator."));
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

            var menuSections = BuildMenuSections(menus.ToList());

            return new CurrentUserResponse(
                user.Id.ToString(),
                user.UserName ?? string.Empty,
                roles.ToArray(),
                permissions,
                menuSections);
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
                user.FullName,
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

            SetRefreshTokenCookie(refreshToken, expiresAt);

            return Result.Success(new TokenResponse(
                accessToken,
                "",
                DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes)));
        }

        /// <summary>
        /// Lưu refresh token vào HttpOnly cookie để trình client không thể truy cập được, chỉ gửi kèm theo yêu cầu đến endpoint làm mới token.
        /// Điều này giúp giảm nguy cơ bị đánh cắp token qua XSS. Cookie được cấu hình với SameSite=Strict để ngăn chặn việc gửi token trong các yêu cầu cross-site, tăng cường bảo mật chống lại CSRF.
        /// Endpoint /api/auth/refresh sẽ kiểm tra cookie này để xác thực và cấp mới access token khi cần thiết.
        /// </summary>
        /// <param name="refreshToken">The refresh token to be stored in the cookie.</param>
        private void SetRefreshTokenCookie(string refreshToken, DateTimeOffset expiresAt)
        {
            httpContextAccessor.HttpContext?.Response.Cookies.Append(
               Constants.REFRESH_TOKEN,
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = expiresAt,
                    Path = "/" // gửi refesh token đên tất cả request
                });
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

        /// <summary>
        /// Gets the menu permissions for the specified role IDs.
        /// </summary>
        /// <param name="roleIds">The role IDs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of menu permission responses.</returns>
        private async Task<IReadOnlyCollection<MenuPermissionResponse>> GetMenuPermissionsAsync(
            IReadOnlyCollection<Guid> roleIds,
            CancellationToken cancellationToken)
        {
            if (roleIds.Count == 0)
            {
                return [];
            }

            var menusByRole = await dbContext.RoleMenuPermissions.Include(x => x.Menu)
               .AsNoTracking()
               .Where(rp => roleIds.Contains(rp.RoleId) && rp.Menu.IsActive)
               .ToListAsync(cancellationToken);

            // Chỉ lấy các menu có quyền read
            // có quyền read thì mới có quyền write , delete
            // không có quyền read thì không có quyền khác, coi như không có quyền vào page đó
            List<string> menus = menusByRole.Where(x => x.PermissionId == PermissionEnum.read.ToString()).Select(x => x.MenuId).Distinct().ToList();

            var rows = menusByRole.Where(x => menus.Contains(x.MenuId))
                .Select(rp => new
                {
                    rp.Menu.MenuId,
                    rp.Menu.MenuName,
                    rp.Menu.MenuShortName,
                    rp.Menu.MenuType,
                    rp.Menu.MenuGroup,
                    rp.Menu.MenuUrl,
                    rp.Menu.ExternalUrl,
                    rp.Menu.IconClass,
                    rp.Menu.BadgeClass,
                    rp.Menu.BadgeText,
                    rp.Menu.Closable,
                    rp.Menu.ParentMenuId,
                    rp.Menu.DisplayOrder,
                    rp.PermissionId
                });

            return rows
                .GroupBy(x => new
                {
                    x.MenuId,
                    x.MenuName,
                    x.MenuShortName,
                    x.MenuType,
                    x.MenuGroup,
                    x.MenuUrl,
                    x.ExternalUrl,
                    x.IconClass,
                    x.BadgeClass,
                    x.BadgeText,
                    x.Closable,
                    x.DisplayOrder,
                    x.ParentMenuId
                })
                .Select(group => new MenuPermissionResponse(
                    group.Key.MenuId,
                    group.Key.MenuName,
                    group.Key.MenuShortName,
                    group.Key.MenuType,
                    group.Key.MenuGroup,
                    group.Key.MenuUrl,
                    group.Key.ExternalUrl,
                    group.Key.IconClass,
                    group.Key.DisplayOrder,
                    group.Key.Closable,
                    group.Key.ParentMenuId,
                    Badge: string.IsNullOrWhiteSpace(group.Key.BadgeText) ? null : new MenuBadgeDto(group.Key.BadgeText, group.Key.BadgeClass),
                    Children: BuildMenuTree(menusByRole, group.Key.MenuId),
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

        /// <summary>
        /// Tìm các menu con của menu hiện tại và xây dựng cây menu đệ quy.
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="parentMenuId"></param>
        /// <returns></returns>
        public static List<MenuPermissionResponse> BuildMenuTree(List<RoleMenuPermission> menus, string? parentMenuId = null)
        {
            return [.. menus
                .Where(x => x.Menu.ParentMenuId == parentMenuId)
                .GroupBy(x => new
                {
                    x.MenuId,
                    x.Menu.MenuName,
                    x.Menu.MenuShortName,
                    x.Menu.MenuType,
                    x.Menu.MenuGroup,
                    x.Menu.MenuUrl,
                    x.Menu.ExternalUrl,
                    x.Menu.IconClass,
                    x.Menu.BadgeClass,
                    x.Menu.BadgeText,
                    x.Menu.Closable,
                    x.Menu.DisplayOrder,
                    x.Menu.ParentMenuId
                })
                .Select(x => new MenuPermissionResponse
                (
                     x.Key.MenuId,
                     x.Key.MenuName,
                     x.Key.MenuShortName,
                     x.Key.MenuType,
                     x.Key.MenuGroup,
                     x.Key.MenuUrl,
                     x.Key.ExternalUrl,
                     x.Key.IconClass,
                     x.Key.DisplayOrder,
                     x.Key.Closable,
                     x.Key.ParentMenuId,
                     Badge : string.IsNullOrWhiteSpace(x.Key.BadgeText)? null: new MenuBadgeDto(text : x.Key.BadgeText,className : x.Key.BadgeClass),
                     Children : BuildMenuTree(menus, x.Key.MenuId),
                     Permissions : [.. x.Select(x => x.PermissionId).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x)],
                     PermissionKeys : [.. x.Select(x => $"{x.MenuId}.{x.PermissionId}").Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x)]
                ))];
        }

        /// <summary>
        /// Group menu theo section (MenuGroup) và sắp xếp theo DisplayOrder, sau đó xây dựng cây menu cho mỗi section.
        /// Trên mobile hoặc menu dọc thường có group theo section.
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public static List<MenuSection> BuildMenuSections(List<MenuPermissionResponse> menus)
        {
            var rootMenus = menus
                .Where(x => x.ParentMenuId == null || x.ParentMenuId == "")
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            return [.. rootMenus
                .GroupBy(x => x.MenuGroup ?? "MAIN MENU")
                .Select(group => new MenuSection
                (
                    Title : group.Key,
                    Items : [.. group
                        .OrderBy(x => x.DisplayOrder)
                        .Select(x => new MenuPermissionResponse
                        (
                             x.Id, // menu Id
                            x.MenuName,
                            x.Title, // short name
                            x.MenuType,
                            x.MenuGroup,
                            x.Path,
                            x.ExternalUrl,
                            x.Icon,
                            x.DisplayOrder,
                            x.Closable,
                            x.ParentMenuId,
                            x.Badge,
                            x.Children,
                            x.Permissions,
                            x.PermissionKeys
                        ))]
                ))];
        }

        /// <summary>
        /// Thu hồi refresh token khi người dùng đăng xuất hoặc khi token bị nghi ngờ bị lộ. Điều này đảm bảo rằng token không còn hợp lệ và không thể được sử dụng để cấp mới access token nữa,
        /// tăng cường bảo mật cho hệ thống.
        /// </summary>
        /// <param name="refreshToken">Refresh token cần thu hồi.</param>
        /// <param name="cancellationToken">Token hủy bỏ để hủy bỏ thao tác nếu cần.</param>
        /// <returns></returns>
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var hashedToken = TokenHasher.HashToken(refreshToken, tokenHashingOptions.Value.Pepper);
            var token = dbContext.RefreshTokens.FirstOrDefault(x => x.TokenHash == hashedToken);
            if (token is not null && token.IsActive)
            {
                token.RevokedAt = DateTimeOffset.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }


        #region Role management : create, update, delete role
        /// <summary>
        /// Tạo role mới
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RoleResponse> CreateRoleAsync(CreateRoleCommand command, CancellationToken cancellationToken)
        {
            var existRole = await roleManager.RoleExistsAsync(command.Name);

            if (existRole)
            {
                throw new Exception($"Role with name '{command.Name}' already exists.");
            }
            var role = new ApplicationRole
            {
                Name = command.Name,
                Description = command.Description
            };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create role: {errors}");
            }

            role = await roleManager.FindByNameAsync(command.Name);

            return new RoleResponse
            (
                role.Id,
                role.Name,
                role.Description
            );
        }

        public async Task<RoleResponse> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(command.Id.ToString());
            if (role is null)
            {
                throw new Exception($"Role with ID '{command.Id}' not found.");
            }

            role.Name = command.Name;
            role.Description = command.Description;
            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update role: {errors}");
            }

            return new RoleResponse
            (
                role.Id,
                role.Name,
                role.Description
            );
        }

        public async Task<bool> DeleteRoleAsync(Guid Id, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(Id.ToString());
            if (role is null)
            {
                throw new Exception($"Role with ID '{Id}' not found.");
            }
            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to delete role: {errors}");
            }
            return true;
        }
        #endregion
    }
}
