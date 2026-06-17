namespace OSM.Application.Features.BaseSetup.RoleMenuPermissions;

public sealed record RoleMenuPermissionResponse(
    string MenuId,
    string MenuName,
    string MenuGroup,
    string? ParentMenuId,
    int DisplayOrder,
    int Level,
    bool IsSelected,
    bool CanRead,
    bool CanWrite,
    bool CanDelete);

public sealed record UpdateRoleMenuPermissionsRequest(
    List<UpdateRoleMenuPermissionItem> Items);

public sealed record UpdateRoleMenuPermissionItem(
    string MenuId,
    bool IsSelected,
    bool CanRead,
    bool CanWrite,
    bool CanDelete);

internal sealed class RoleMenuPermissionSqlRow
{
    public string MenuId { get; set; } = string.Empty;

    public string MenuName { get; set; } = string.Empty;

    public string MenuGroup { get; set; } = string.Empty;

    public string? ParentMenuId { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsSelected { get; set; }

    public bool CanRead { get; set; }

    public bool CanWrite { get; set; }

    public bool CanDelete { get; set; }
}

internal static class RoleMenuPermissionTreeBuilder
{
    public static List<RoleMenuPermissionResponse> Build(IEnumerable<RoleMenuPermissionSqlRow> sourceRows)
    {
        var rows = sourceRows
            .Select(Normalize)
            .GroupBy(x => x.MenuId, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .ToList();

        var rowById = rows.ToDictionary(x => x.MenuId, StringComparer.OrdinalIgnoreCase);

        var childrenByParentId = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.ParentMenuId) && rowById.ContainsKey(x.ParentMenuId!))
            .GroupBy(x => x.ParentMenuId!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                x => x.Key,
                x => x.OrderBy(menu => menu.DisplayOrder).ThenBy(menu => menu.MenuName).ToList(),
                StringComparer.OrdinalIgnoreCase);

        var roots = rows
            .Where(x => string.IsNullOrWhiteSpace(x.ParentMenuId) || !rowById.ContainsKey(x.ParentMenuId!))
            .OrderBy(x => x.MenuGroup)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.MenuName)
            .ToList();

        var result = new List<RoleMenuPermissionResponse>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var root in roots)
        {
            AddNode(root, level: 0, childrenByParentId, result, visited);
        }

        // Safety net: if bad data creates orphan/cycle-like leftovers, still show them.
        foreach (var row in rows.OrderBy(x => x.MenuGroup).ThenBy(x => x.DisplayOrder).ThenBy(x => x.MenuName))
        {
            if (visited.Add(row.MenuId))
            {
                result.Add(ToResponse(row, level: 0));
            }
        }

        return result;
    }

    private static void AddNode(
        RoleMenuPermissionSqlRow row,
        int level,
        IReadOnlyDictionary<string, List<RoleMenuPermissionSqlRow>> childrenByParentId,
        List<RoleMenuPermissionResponse> result,
        HashSet<string> visited)
    {
        if (!visited.Add(row.MenuId))
        {
            return;
        }

        result.Add(ToResponse(row, level));

        if (!childrenByParentId.TryGetValue(row.MenuId, out var children))
        {
            return;
        }

        foreach (var child in children)
        {
            AddNode(child, level + 1, childrenByParentId, result, visited);
        }
    }

    private static RoleMenuPermissionResponse ToResponse(RoleMenuPermissionSqlRow row, int level)
    {
        var isSelected = row.IsSelected || row.CanRead || row.CanWrite || row.CanDelete;

        return new RoleMenuPermissionResponse(
            row.MenuId,
            row.MenuName,
            row.MenuGroup,
            row.ParentMenuId,
            row.DisplayOrder,
            level,
            isSelected,
            row.CanRead,
            row.CanWrite,
            row.CanDelete);
    }

    private static RoleMenuPermissionSqlRow Normalize(RoleMenuPermissionSqlRow row)
    {
        row.ParentMenuId = string.IsNullOrWhiteSpace(row.ParentMenuId) ? null : row.ParentMenuId;
        row.MenuGroup = row.MenuGroup ?? string.Empty;
        row.MenuName = row.MenuName ?? row.MenuId;
        row.IsSelected = row.IsSelected || row.CanRead || row.CanWrite || row.CanDelete;
        return row;
    }
}
