using MediatR;
using Microsoft.EntityFrameworkCore;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;
using OSM.Application.Common.Errors;
using OSM.Domain.Entities.Products;


namespace OSM.Application.Features.Products.CreateProduct
{
    public sealed class CreateProductCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim().ToUpperInvariant();
            var exists = await dbContext.Products.AnyAsync(x => x.Code == code && !x.IsDeleted, cancellationToken);
            if (exists)
            {
                return Result.Failure<Guid>(new Error("Products.CodeDuplicated", "Product code already exists.",ErrorType.Conflict));
            }

            var product = Product.Create(request.Name, code, request.StockQuantity);
            dbContext.Products.Add(product);

            return Result.Success(product.Id);
        }
    }
}
