using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Events.Notifications
{
    public interface INotificationHandler<TNotification>
     where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken ct);
    }

}
