using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.UserSetup.GetUsers
{
    public sealed class GetUsersQueryHandler(IDapperHelper DapperHelper) : IRequestHandler<GetUsersQuery, Result<List<UserResponse>>>
    {
        public async Task<Result<List<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            string sql = @"SELECT 
                                 [UserName] as UserId,
                                 [FullName],
                                 [FullName] as UserName,
                                 [Email], 
                                 [PasswordShow],
                                 R.Name AS Role,
                                 [Department], 
                                 [IsActive], 
                                 [CreatedAt], 
                                 [CreatedBy], 
                                 [ModifiedAt],
                                 [ModifiedBy] 
                            	  
                           FROM [dbo].[AspNetUsers]	U
                           INNER JOIN [dbo].[AspNetUserRoles] UR ON U.Id = UR.UserId
                           INNER JOIN [dbo].[AspNetRoles] R ON UR.RoleId = R.Id";

            var users = await DapperHelper.QueryAsync<UserResponse>(sql);

            return Result.Success(users.ToList());
        }
    }
}
