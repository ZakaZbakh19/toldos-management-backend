using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Events
{
    public interface INotificationPublisher
    {
        Task Publish(INotification notification, CancellationToken ct);
    }
}
