using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Products.Commands.CreateProduct;
using ModularBackend.Application.Products.Queries.GetProductById;
using System.Reflection;

namespace ModularBackend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<IRequestHandler<CreateProductCommand, Guid>, CreateProductCommandHandler>();
            services.AddScoped<IRequestHandler<GetProductByIdQuery, ProductDetailDTO>, GetProductByIdHandler>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
