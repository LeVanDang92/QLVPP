using MediatR;
using Microsoft.EntityFrameworkCore;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.Products.UpdateProduct
{
    public sealed class UpdateProductCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateProductCommand, Result>
    {
        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);

            if (product is null)
            {
                return Result.Failure(new Error($"Products :{request.Id}", "Product not found."));
            }

            product.Update(request.Name, request.StockQuantity, request.isActive);

            return Result.Success();
        }
    }
}
