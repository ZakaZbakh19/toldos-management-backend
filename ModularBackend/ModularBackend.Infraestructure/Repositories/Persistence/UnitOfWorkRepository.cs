using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Infraestructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infraestructure.Repositories.Persistance
{
    public class UnitOfWorkRepository : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWorkRepository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _dbContext.SaveChangesAsync(ct);
        }
    }
}
