using ModularBackend.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ModularBackend.Application.Abstractions.Common
{
    public interface IReadRepository<T> 
        where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    }
}
