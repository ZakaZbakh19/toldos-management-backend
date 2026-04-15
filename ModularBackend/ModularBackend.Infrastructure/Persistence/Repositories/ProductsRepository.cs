using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Domain.Entities;
using ModularBackend.Infrastructure.Persistence.Repositories.Common;

namespace ModularBackend.Infrastructure.Persistence.Repositories
{
    public class ProductsRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductsRepository(ApplicationDbContext db)
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
