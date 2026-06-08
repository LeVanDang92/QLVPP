using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.RoleSetup.GetRoles
{
    public sealed class GetRoleQueryHandler(IDapperHelper dapperHelper) : IRequestHandler<GetRoleQuery, Result<List<RoleResponse>>>
    {
        public async Task<Result<List<RoleResponse>>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            string sql = "SELECT Id, Name FROM AspNetRoles";

            var roles = await dapperHelper.QueryAsync<RoleResponse>(sql);

            return Result.Success(roles.ToList());
        }
    }
}
