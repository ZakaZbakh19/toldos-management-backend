using Microsoft.Extensions.DependencyInjection;
using ModularBackend.Application.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Events
{
    public sealed class NotificationPublisher : INotificationPublisher
    {
        private readonly IServiceProvider _sp;
        public NotificationPublisher(IServiceProvider sp) => _sp = sp;
        public async Task Publish(INotification notification, CancellationToken ct)
        {
            var handlers = _sp.GetServices(
            typeof(INotificationHandler<>)
            .MakeGenericType(notification.GetType()));
            foreach (dynamic handler in handlers)
            {
                await handler.Handle((dynamic)notification, ct);
            }
        }
    }

}
