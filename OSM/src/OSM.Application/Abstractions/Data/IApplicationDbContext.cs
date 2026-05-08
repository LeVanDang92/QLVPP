using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OSM.Domain.Common;
using OSM.Domain.Entities.Products;

namespace OSM.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    IReadOnlyCollection<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}