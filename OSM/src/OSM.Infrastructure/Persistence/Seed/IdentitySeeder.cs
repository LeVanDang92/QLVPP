using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OSM.Infrastructure.Common;
using OSM.Infrastructure.Identity;
using OSM.Infrastructure.Persistence.Configurations;

namespace OSM.Infrastructure.Persistence.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IdentitySeedOptions options)
        {
            await SeedRolesAsync(roleManager);
            await SeedMenusAsync(dbContext);
            await SeedRolePermissionsAsync(dbContext, roleManager);
            await SeedAdminUserAsync(userManager, options);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roles = ["Admin", "User"];

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName
                    });
                }
            }
        }

        private static async Task SeedRolePermissionsAsync(
            ApplicationDbContext dbContext,
            RoleManager<ApplicationRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            var userRole = await roleManager.FindByNameAsync("User");

            if (adminRole is null || userRole is null)
                return;

            await AddMissingRolePermissionsAsync(dbContext, adminRole.Id, Constants.MENU_ID_00, PermissionCodes.All);

            await AddMissingRolePermissionsAsync(dbContext, userRole.Id, Constants.MENU_ID_00, new[] { PermissionCodes.Read });

            await dbContext.SaveChangesAsync();
        }

        private static async Task AddMissingRolePermissionsAsync(
            ApplicationDbContext dbContext,
            Guid roleId, string menuId,
            string[] permissionIds)
        {
            var existingPermissionIds = await dbContext.RoleMenuPermissions
                .Where(x => x.RoleId == roleId && x.MenuId == menuId)
                .Select(x => x.PermissionId)
                .ToListAsync();

            var missingPermissionIds = permissionIds
                .Where(id => !existingPermissionIds.Contains(id));

            foreach (var permissionId in missingPermissionIds)
            {
                dbContext.RoleMenuPermissions.Add(new RoleMenuPermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    MenuId = Constants.MENU_ID_00
                });
            }
        }

        private static async Task SeedMenusAsync(ApplicationDbContext dbContext)
        {
            if (await dbContext.Menus.AnyAsync(x => x.MenuId == Constants.MENU_ID_00))
            {
                return;
            }

            dbContext.Menus.Add(new Menus
            {
                MenuId = Constants.MENU_ID_00,
                MenuName = "Base Menu",
                MenuShortName = "Base Menu",
                MenuType = Constants.MENU_TYPE_M,
                MenuGroup = Constants.MENU_GROUP,
                MenuUrl = null,
                IconIndex = "0",
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "seed"
            });

            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IdentitySeedOptions options)
        {
            var admin = await userManager.FindByNameAsync(options.AdminUserName);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = options.AdminUserName,
                    Email = options.AdminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, options.AdminPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(x => x.Description));

                    throw new InvalidOperationException(errors);
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}