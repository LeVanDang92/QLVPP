using OSM.Application.Abstractions.Messaging;

namespace OSM.Application.Features.Products.CreateProduct
{
    /// <summary>
    /// Đây là dữ liệu client gửi lên để tạo Product.Kết quả trả về là Guid của Product mới.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Code"></param>
    /// <param name="StockQuantity"></param>
    public sealed record CreateProductCommand(string Name,string Code,int StockQuantity) : ICommand<Guid>;
}
