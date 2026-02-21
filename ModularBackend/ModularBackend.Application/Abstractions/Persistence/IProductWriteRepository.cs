using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence
{
    public interface IProductWriteRepository : IWriteRepository<Product>
    {
    }
}
