using ModularBackend.Application.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Events
{
    public record UserRegisteredNotification(string name, string email) : INotification;
}
