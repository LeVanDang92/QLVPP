using MediatR;
using Microsoft.EntityFrameworkCore;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.Products.DeleteProduct
{
    public sealed class DeleteProductCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteProductCommand, Result>
    {
        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);

            if (product is null)
            {
                return Result.Failure(new Error($"Products :{request.Id}", "Product not found."));
            }

            product.Delete();

            return Result.Success();
        }
    }
}
