using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Products.Queries.GetProductById;
using ModularBackend.Domain.Entities;
using ModularBackend.Infraestructure.Persistance;
using ModularBackend.Infraestructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ModularBackend.Infraestructure.Repositories
{
    public class ProductReadRepository : IProductQuery
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ProductReadRepository(ApplicationDbContext applicationDbContext)
        { 
            _applicationDbContext = applicationDbContext;
        }

        public Task<ProductDetailDTO?> FindAsync(Expression<Func<ProductDetailDTO, bool>> predicate, CancellationToken ct = default)
        {
            return _applicationDbContext.Products
                .AsNoTracking()
                .Select(p => new ProductDetailDTO
                (
                    p.Id,
                    p.Name,
                    p.Description,
                    p.BasePrice.Amount
                ))
                .FirstOrDefaultAsync(predicate, ct);
        }

        public Task<ProductDetailDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _applicationDbContext.Products
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

        public Task<List<ProductDetailDTO>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return _applicationDbContext.Products
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
        }
    }
}
