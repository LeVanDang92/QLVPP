using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;
using OSM.Application.Common.Errors;
using OSM.Application.Features.BaseSetup.RoleMenuPermissions;

namespace OSM.Application.Features.BaseSetup.RoleMenuPermissions.GetRoleMenuPermissions;

public sealed class GetRoleMenuPermissionsQueryHandler(IDapperHelper dapperHelper)
    : IRequestHandler<GetRoleMenuPermissionsQuery, Result<List<RoleMenuPermissionResponse>>>
{
    public async Task<Result<List<RoleMenuPermissionResponse>>> Handle(
        GetRoleMenuPermissionsQuery request,
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

        const string sql = @"
SELECT
    m.MenuId,
    m.MenuName,
    ISNULL(m.MenuGroup, '') AS MenuGroup,
    NULLIF(m.ParentMenuId, '') AS ParentMenuId,
    m.DisplayOrder,
    CAST(CASE WHEN COUNT(rmp.PermissionId) > 0 THEN 1 ELSE 0 END AS bit) AS IsSelected,
    CAST(CASE WHEN SUM(CASE WHEN rmp.PermissionId = 'read' THEN 1 ELSE 0 END) > 0 THEN 1 ELSE 0 END AS bit) AS CanRead,
    CAST(CASE WHEN SUM(CASE WHEN rmp.PermissionId = 'write' THEN 1 ELSE 0 END) > 0 THEN 1 ELSE 0 END AS bit) AS CanWrite,
    CAST(CASE WHEN SUM(CASE WHEN rmp.PermissionId = 'delete' THEN 1 ELSE 0 END) > 0 THEN 1 ELSE 0 END AS bit) AS CanDelete
FROM Menus m
LEFT JOIN RoleMenuPermission rmp
    ON rmp.MenuId = m.MenuId
   AND rmp.RoleId = @RoleId
WHERE m.IsActive = 1
GROUP BY
    m.MenuId,
    m.MenuName,
    m.MenuGroup,
    m.ParentMenuId,
    m.DisplayOrder;";

        var rows = await dapperHelper.QueryAsync<RoleMenuPermissionSqlRow>(sql, new { request.RoleId });
        var result = RoleMenuPermissionTreeBuilder.Build(rows);

        return Result.Success(result);
    }
}
