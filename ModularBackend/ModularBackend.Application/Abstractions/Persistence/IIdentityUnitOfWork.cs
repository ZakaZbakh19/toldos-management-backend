using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence
{
    public interface IIdentityUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
