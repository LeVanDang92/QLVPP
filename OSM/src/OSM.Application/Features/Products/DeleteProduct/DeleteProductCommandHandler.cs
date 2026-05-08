using MediatR;
using Microsoft.EntityFrameworkCore;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;
using OSM.Application.Common.Errors;

namespace OSM.Application.Features.Products.DeleteProduct
{
    public sealed class DeleteProductCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteProductCommand, Result>
    {
        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

            if (product is null)
            {
                return Result.Failure(new Error($"Products :{request.Id}", "Product not found.", ErrorType.NotFound));
            }

            product.Delete();

            return Result.Success();
        }
    }
}
