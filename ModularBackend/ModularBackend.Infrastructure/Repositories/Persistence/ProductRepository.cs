using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Domain.Entities;
using ModularBackend.Infrastructure.Persistance.Context;
using ModularBackend.Infrastructure.Repositories.Common;

namespace ModularBackend.Infrastructure.Repositories.Persistence
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext db)
            : base(db)
        {
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _set.AddAsync(product, cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _set.FindAsync(productId, cancellationToken);
        }
    }
}
