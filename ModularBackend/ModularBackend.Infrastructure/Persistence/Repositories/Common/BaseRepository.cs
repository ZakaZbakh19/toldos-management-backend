using Microsoft.EntityFrameworkCore;
using ModularBackend.Domain.Abstractions;
using ModularBackend.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistence.Repositories.Common
{
    public abstract class BaseRepository<TEntity>
    where TEntity : Entity
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<TEntity> _set;

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = dbContext.Set<TEntity>();
        }
    }
}
