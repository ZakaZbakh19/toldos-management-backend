using ModularBackend.Application.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Events
{
    public sealed class UserRegisteredHandler
        : INotificationHandler<UserRegisteredNotification>
    {
        public Task Handle(UserRegisteredNotification notification, CancellationToken ct)
        {
            

            return Task.CompletedTask;
        }
    }

}
