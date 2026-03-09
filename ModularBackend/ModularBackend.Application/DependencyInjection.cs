using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Behaviors;
using ModularBackend.Application.Products.Commands.CreateProduct;
using ModularBackend.Application.Products.Queries.GetProductById;
using ModularBackend.Application.Users.Commands.Auth;
using ModularBackend.Application.Users.Commands.Auth.Login;
using ModularBackend.Application.Users.Commands.Auth.Logout;
using ModularBackend.Application.Users.Commands.Auth.Refresh;
using ModularBackend.Application.Users.Commands.Auth.Register;
using ModularBackend.Application.Users.Commands.Register;
using System.Reflection;

namespace ModularBackend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddScoped<IMediator, Mediator>();
            services.AddHandlersFromAssembly(assembly);
            services.AddValidatorsFromAssembly(assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            return services;
        }

        private static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var handlerInterfaceType = typeof(IRequestHandler<,>);

            var handlerTypes = assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .Select(t => new
                {
                    Implementation = t,
                    Contracts = t.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                        .ToArray()
                })
                .Where(x => x.Contracts.Length > 0);

            foreach (var handlerType in handlerTypes)
            {
                foreach (var contract in handlerType.Contracts)
                {
                    services.AddScoped(contract, handlerType.Implementation);
                }
            }

            return services;
        }
    }
}
