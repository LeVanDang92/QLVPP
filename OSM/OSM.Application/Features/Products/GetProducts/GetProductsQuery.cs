using OSM.Application.Abstractions.Messaging;
using OSM.Application.Common;

namespace OSM.Application.Features.Products.GetProducts
{
    public sealed record GetProductsQuery(string? Search, int PageIndex= 1,int PageSize = 20) :IQuery<PagedResult<ProductResponse>>;
}
