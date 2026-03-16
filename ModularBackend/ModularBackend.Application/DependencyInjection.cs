using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Abstractions.Events;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Behaviors;
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
            var requestHandlerType = typeof(IRequestHandler<,>);
            var notificationHandlerType = typeof(INotificationHandler<>);

            var types = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface);

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();

                foreach (var contract in interfaces)
                {
                    if (contract.IsGenericType)
                    {
                        var definition = contract.GetGenericTypeDefinition();

                        if (definition == requestHandlerType || definition == notificationHandlerType)
                        {
                            services.AddScoped(contract, type);
                        }
                    }
                }
            }

            return services;
        }
    }
}
