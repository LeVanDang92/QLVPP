using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.MenuSetup.GetMenu
{
    public sealed class GetMenuQueryHandler(IDapperHelper dapperHelper) : IRequestHandler<GetMenuQuery, Result<List<MenuResponse>>>
    {
        public async Task<Result<List<MenuResponse>>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
        {
            string sql = @"SELECT [MenuId]
                                 ,[MenuName]
                                 ,[MenuShortName]
                                 ,[MenuType]
                                 ,[MenuGroup]
                                 ,[MenuUrl]
                            	 ,[ExternalUrl]
                                 ,[IconClass]
                            	 ,[DisplayOrder]
                            	 ,[BadgeText]
                                 ,[BadgeClass]
                                 ,[ParentMenuId]
                                 ,[Closable]
                                 ,[IsActive]
                             FROM [dbo].[Menus]
                             ORDER BY DisplayOrder";

            var menuList = await dapperHelper.QueryAsync<MenuResponse>(sql);

            return Result.Success(menuList.ToList());
        }
    }
}
