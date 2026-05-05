using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Products.GetProductById
{
    public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
}
