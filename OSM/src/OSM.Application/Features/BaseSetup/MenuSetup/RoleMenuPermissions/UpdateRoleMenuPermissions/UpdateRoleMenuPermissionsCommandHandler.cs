using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;
using OSM.Application.Common.Errors;
using System.Data;

namespace OSM.Application.Features.BaseSetup.RoleMenuPermissions.UpdateRoleMenuPermissions;

public sealed class UpdateRoleMenuPermissionsCommandHandler(
    IDapperHelper dapperHelper,
    ISqlConnectionFactory sqlConnectionFactory)
    : IRequestHandler<UpdateRoleMenuPermissionsCommand, Result<List<RoleMenuPermissionResponse>>>
{
    private const string ReadPermission = "read";
    private const string WritePermission = "write";
    private const string DeletePermission = "delete";

    public async Task<Result<List<RoleMenuPermissionResponse>>> Handle(
        UpdateRoleMenuPermissionsCommand request,
        CancellationToken cancellationToken)
    {
        var roleExists = await dapperHelper.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(1) FROM AspNetRoles WHERE Id = @RoleId",
            new { request.RoleId });

        if (roleExists == 0)
        {
            return Result.Failure<List<RoleMenuPermissionResponse>>(
                Error.NotFound("Role.NotFound", "Role does not exist."));
        }

        var menus = (await dapperHelper.QueryAsync<RoleMenuPermissionSqlRow>(@"
                            SELECT
                                MenuId,
                                MenuName,
                                ISNULL(MenuGroup, '') AS MenuGroup,
                                NULLIF(ParentMenuId, '') AS ParentMenuId,
                                DisplayOrder,
                                CAST(0 AS bit) AS IsSelected,
                                CAST(0 AS bit) AS CanRead,
                                CAST(0 AS bit) AS CanWrite,
                                CAST(0 AS bit) AS CanDelete
                            FROM Menus
                            WHERE IsActive = 1;"))
                    .ToDictionary(x => x.MenuId, StringComparer.OrdinalIgnoreCase);

        var invalidMenuIds = request.Items
            .Select(x => x.MenuId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(x => !menus.ContainsKey(x))
            .ToList();

        if (invalidMenuIds.Count > 0)
        {
            return Result.Failure<List<RoleMenuPermissionResponse>>(
                Error.Validation(invalidMenuIds
                    .Select(menuId => new ValidationError("menuId", $"Menu '{menuId}' does not exist or is inactive."))
                    .ToList()));
        }

        var permissionsByMenuId = NormalizePermissions(request.Items, menus);
        var permissionRows = BuildPermissionRows(request.RoleId, permissionsByMenuId).DistinctBy(x => new { x.RoleId, x.MenuId, x.PermissionId }).ToList();

        await dapperHelper.ExecuteAsync("DELETE FROM RoleMenuPermission WHERE RoleId = @RoleId", new { request.RoleId });

        if (permissionRows.Count > 0)
        {
            await dapperHelper.ExecuteAsync(@"
                    INSERT INTO RoleMenuPermission (RoleId, MenuId, PermissionId)
                    VALUES (@RoleId, @MenuId, @PermissionId);",
                permissionRows);
        }

        foreach (var menu in menus.Values)
        {
            if (!permissionsByMenuId.TryGetValue(menu.MenuId, out var permissionSet))
            {
                continue;
            }

            menu.IsSelected = permissionSet.Count > 0;
            menu.CanRead = permissionSet.Contains(ReadPermission);
            menu.CanWrite = permissionSet.Contains(WritePermission);
            menu.CanDelete = permissionSet.Contains(DeletePermission);
        }

        return Result.Success(RoleMenuPermissionTreeBuilder.Build(menus.Values));
    }

    private static Dictionary<string, HashSet<string>> NormalizePermissions(
        IEnumerable<UpdateRoleMenuPermissionItem> items,
        IReadOnlyDictionary<string, RoleMenuPermissionSqlRow> menus)
    {
        var permissionsByMenuId = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.MenuId) || !menus.ContainsKey(item.MenuId))
            {
                continue;
            }

            // Rule: write/delete imply read. A menu is persisted only when read is true after normalization.
            var canRead = item.CanRead || item.CanWrite || item.CanDelete;

            if (!canRead)
            {
                continue;
            }

            AddPermission(permissionsByMenuId, item.MenuId, ReadPermission);

            if (item.CanWrite)
            {
                AddPermission(permissionsByMenuId, item.MenuId, WritePermission);
            }

            if (item.CanDelete)
            {
                AddPermission(permissionsByMenuId, item.MenuId, DeletePermission);
            }

            AddParentReadPermissions(permissionsByMenuId, menus, item.MenuId);
        }

        return permissionsByMenuId;
    }

    private static void AddParentReadPermissions(
        Dictionary<string, HashSet<string>> permissionsByMenuId,
        IReadOnlyDictionary<string, RoleMenuPermissionSqlRow> menus,
        string menuId)
    {
        var currentMenu = menus[menuId];
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        while (!string.IsNullOrWhiteSpace(currentMenu.ParentMenuId)
            && menus.TryGetValue(currentMenu.ParentMenuId, out var parentMenu)
            && visited.Add(parentMenu.MenuId))
        {
            AddPermission(permissionsByMenuId, parentMenu.MenuId, ReadPermission);
            currentMenu = parentMenu;
        }
    }

    private static void AddPermission(
        Dictionary<string, HashSet<string>> permissionsByMenuId,
        string menuId,
        string permission)
    {
        if (!permissionsByMenuId.TryGetValue(menuId, out var permissions))
        {
            permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            permissionsByMenuId[menuId] = permissions;
        }

        permissions.Add(permission);
    }

    private static List<RoleMenuPermissionRow> BuildPermissionRows(
        Guid roleId,
        Dictionary<string, HashSet<string>> permissionsByMenuId)
    {
        return permissionsByMenuId
            .SelectMany(menu => menu.Value.Select(permission => new RoleMenuPermissionRow(roleId, menu.Key, permission)))
            .ToList();
    }

    private sealed record RoleMenuPermissionRow(Guid RoleId, string MenuId, string PermissionId);
}
