using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Events.IntegrationEvents
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent integrationEvent, CancellationToken cancellationToken);
    }
}
