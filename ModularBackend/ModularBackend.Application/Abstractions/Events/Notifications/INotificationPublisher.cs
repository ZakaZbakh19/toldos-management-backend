using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Events.Notifications
{
    public interface INotificationPublisher
    {
        Task Publish(INotification notification, CancellationToken ct);
    }
}
