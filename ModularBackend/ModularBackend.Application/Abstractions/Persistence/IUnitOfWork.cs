using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistance
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

}
