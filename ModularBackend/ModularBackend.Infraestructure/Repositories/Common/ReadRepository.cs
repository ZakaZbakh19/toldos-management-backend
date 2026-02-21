using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Infraestructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ModularBackend.Infraestructure.Repositories.Common
{
    public abstract class ReadRepository<T> : IReadRepository<T> where T : AggregateRoot
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReadRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return _applicationDbContext
                .Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, ct);
        }

        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _applicationDbContext
                .Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public Task<List<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be >= 1.");

            return _applicationDbContext
                .Set<T>()
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }
    }
}
