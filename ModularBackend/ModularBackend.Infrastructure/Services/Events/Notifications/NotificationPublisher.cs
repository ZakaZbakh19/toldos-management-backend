using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Abstractions.Events.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Services.Events.Notifications
{
    public sealed class NotificationPublisher : INotificationPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("Handle")
                             ?? throw new InvalidOperationException("Handle method not found.");

                var task = (Task?)method.Invoke(handler, new object[] { notification, cancellationToken });
                if (task is null)
                    throw new InvalidOperationException("Handler returned null task.");

                await task;
            }
        }
    }

}
