using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Infraestructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infraestructure.Repositories.Common
{
    public abstract class WriteRepository<T> : IWriteRepository<T>
        where T : Domain.Abstractions.AggregateRoot
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public WriteRepository(ApplicationDbContext applicationDbContext,
            IUnitOfWork unitOfWork)
        {
            _dbContext = applicationDbContext;
             _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbContext.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _dbContext.Remove(entity);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _dbContext.Set<T>().FindAsync(new object[] { id }, ct);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Remove(entity);

            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }
        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _dbContext.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
