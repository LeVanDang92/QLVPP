namespace OSM.Application.Features.Products
{
    public sealed record ProductResponse(
        Guid Id,
        string Name,
        string Code,
        int StockQuantity,
        bool IsActive,
        DateTimeOffset CreatedAt);
}
