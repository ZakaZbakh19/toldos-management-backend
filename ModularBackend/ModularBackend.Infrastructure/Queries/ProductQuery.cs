using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Product;
using ModularBackend.Application.Products.Queries.Common;
using ModularBackend.Domain.Entities;
using ModularBackend.Infrastructure.Persistance.Context;
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

        //public Task<ProductDetailDTO?> FindAsync(Expression<Func<ProductDetailDTO, bool>> predicate, CancellationToken ct = default)
        //{
        //    return _applicationDbContext.Products
        //        .AsNoTracking()
        //        .Select(p => new ProductDetailDTO
        //        (
        //            p.Id,
        //            p.Name,
        //            p.Description,
        //            p.BasePrice.Amount
        //        ))
        //        .FirstOrDefaultAsync(predicate, ct);
        //}

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

        public async Task<PagedResult<ProductDetailDTO>?> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var items = await _context.Products
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDetailDTO
                (
                    p.Id,
                    p.Name,
                    p.Description,
                    p.BasePrice.Amount
                ))
                .ToListAsync(ct);

            return new PagedResult<ProductDetailDTO>()
            {
                Items = items,
                TotalCount = items.Count,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
