using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Domain.Entities;
using ModularBackend.Infraestructure.Persistance;
using ModularBackend.Infraestructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infraestructure.Repositories
{
    public class ProductWriteRepository : WriteRepository<Product>, IProductWriteRepository
    {
        public ProductWriteRepository(ApplicationDbContext db, IUnitOfWork uow)
            : base(db, uow)
        {
        }
    }
}
