using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Products.DeleteProduct
{
    public sealed record DeleteProductCommand(Guid Id) : ICommand;
}
