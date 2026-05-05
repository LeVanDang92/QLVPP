using Dapper;
using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.Products.GetProducts
{
    public sealed class GetProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<GetProductsQuery,Result<PagedResult<ProductResponse>>>
    {
        public async Task<Result<PagedResult<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
           
            var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
            var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);
            var search = string.IsNullOrWhiteSpace(request.Search) ? null : $"%{request.Search.Trim()}%";

            const string sql = """
                            SELECT Id, Name, Code, StockQuantity, IsActive, CreatedAt
                            FROM Products
                            WHERE IsDeleted = 0
                              AND (@Search IS NULL OR Name LIKE @Search OR Code LIKE @Search)
                            ORDER BY CreatedAt DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                            SELECT COUNT(1)
                            FROM Products
                            WHERE IsDeleted = 0
                              AND (@Search IS NULL OR Name LIKE @Search OR Code LIKE @Search);
                           """;

            var connection = sqlConnectionFactory.CreateConnection();
            using var multi =await  connection.QueryMultipleAsync(sql, new { Offset = (pageIndex - 1) * pageSize, PageSize = pageSize, Search = search });
            var products = (await multi.ReadAsync<ProductResponse>()).ToList();
            var total = await multi.ReadSingleAsync<int>();

            return Result.Success(new PagedResult<ProductResponse>(products, pageIndex, pageSize, total));
        }
    }
}
