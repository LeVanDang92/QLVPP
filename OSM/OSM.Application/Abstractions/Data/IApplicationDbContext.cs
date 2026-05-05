using Microsoft.EntityFrameworkCore;
using OSM.Domain.Entities.Products;

namespace OSM.Application.Abstractions.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
