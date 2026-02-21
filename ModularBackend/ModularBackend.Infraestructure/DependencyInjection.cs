using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Products.Commands.CreateProduct;
using ModularBackend.Application.Products.Queries.GetProductById;
using ModularBackend.Infraestructure.Persistance;
using ModularBackend.Infraestructure.Repositories;
using ModularBackend.Infraestructure.Repositories.Persistance;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModularBackend.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
         IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DevConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IProductQuery, ProductReadRepository>();

            return services;
        }
    }
}
