using ModularBackend.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Common
{
    public interface IWriteRepository<TEntity>
        where TEntity : AggregateRoot
    {
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        Task UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task DeleteAsync(TEntity entity, CancellationToken ct = default);
        Task<bool> DeleteByIdAsync(Guid id, CancellationToken ct = default);
    }

}
