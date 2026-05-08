using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Products.UpdateProduct
{
    public sealed record UpdateProductCommand(Guid Id, string Name, int StockQuantity,bool IsActive) : ICommand;
}
