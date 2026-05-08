namespace OSM.Application.Features.Products
{
    public sealed record UpdateProductRequest(string Name, int StockQuantity, bool IsActive);
}
