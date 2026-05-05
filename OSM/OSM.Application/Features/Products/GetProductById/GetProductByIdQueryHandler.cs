using MediatR;
using Microsoft.EntityFrameworkCore;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.Products.GetProductById
{
    public sealed class GetProductByIdQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
    {
        public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.AsNoTracking().Where(p => p.Id == request.Id && !p.IsDeleted)
                .Select(p => new ProductResponse (p.Id,p.Name,p.Code,p.StockQuantity,p.IsActive,p.CreatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return Result.Failure<ProductResponse>(new Error("ProductNotFound", $"Product with ID {request.Id} not found."));
            }

            return Result.Success(product);
        }
    }
}
