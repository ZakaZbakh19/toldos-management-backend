using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Infraestructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Repositories.Persistence
{
    public class IdentityUnitOfWorkRepository : IIdentityUnitOfWork
    {
        private readonly IdentityDbContext _dbContext;

        public IdentityUnitOfWorkRepository(IdentityDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _dbContext.SaveChangesAsync(ct);
        }
    }
}
