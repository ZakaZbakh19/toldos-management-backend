using ModularBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Persistence.Products
{
    public interface IProductRepository
    {
        Task AddAsync(ModularBackend.Domain.Entities.Product product, CancellationToken cancellationToken = default);
    }
}
