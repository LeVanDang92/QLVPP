using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSM.Application.Features.Products;
using OSM.Application.Features.Products.CreateProduct;
using OSM.Application.Features.Products.DeleteProduct;
using OSM.Application.Features.Products.GetProductById;
using OSM.Application.Features.Products.GetProducts;
using OSM.Application.Features.Products.UpdateProduct;
using OSM.Infrastructure.Authorization;

namespace OSM.API.Controllers
{
    [ApiVersion("1.0")]
    public sealed class ProductsController(ISender sender) : ApiController
    {
        [HttpGet]
        [Authorize(Policy = Policies.ProductRead)]
        public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(new GetProductsQuery(search, pageIndex, pageSize), cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = Policies.ProductRead)]
        public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Policy = Policies.ProductWrite)]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetProductById), new { id = result.Value, version = "1.0" }, result.Value) : HandleResult(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.ProductWrite)]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateProductCommand(id, request.Name, request.StockQuantity, request.IsActive), cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.ProductDelete)]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);
            return HandleResult(result);
        }
    }
}
