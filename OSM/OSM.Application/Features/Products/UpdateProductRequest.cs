namespace OSM.Application.Features.Products
{
    public sealed record UpdateProductRequest(string Name, decimal Price, int StockQuantity, bool IsActive);
}
