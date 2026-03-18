using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Product;
using ModularBackend.Application.Products.Queries.Common;
using ModularBackend.Application.Products.Queries.GetProducts;
using ModularBackend.Domain.Entities;
using ModularBackend.Infrastructure.Persistance.Context;
using ModularBackend.Infrastructure.Queries.Extensions;
using ModularBackend.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ModularBackend.Infrastructure.Queries
{
    public class ProductQuery : IProductQuery
    {
        private readonly ApplicationDbContext _context;
        public ProductQuery(ApplicationDbContext db)
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
                .ApplyFilters(request);

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
