using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Features.Products.Common;
using ModularBackend.Application.Features.Products.GetProducts;
using ModularBackend.Application.Shared;
using ModularBackend.Infrastructure.Persistence;
using ModularBackend.Infrastructure.Persistence.Queries.Extensions;

namespace ModularBackend.Infrastructure.Persistence.Queries
{
    public class ProductsQuery : IProductQuery
    {
        private readonly ApplicationDbContext _context;
        public ProductsQuery(ApplicationDbContext db)
        {
            _context = db;
        }

        public Task<ProductDetailDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context
                .Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailDTO
                (
                    p.Id,
                    p.Name,
                    p.Description,
                    p.BasePrice.Amount
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResult<ProductDetailDTO>> GetPagedAsync(GetProductsQuery request, CancellationToken ct = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .ApplyFilters(request)
                .ApplySorting(request);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductDetailDTO(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.BasePrice.Amount
                ))
                .ToListAsync(ct);

            return new PagedResult<ProductDetailDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
