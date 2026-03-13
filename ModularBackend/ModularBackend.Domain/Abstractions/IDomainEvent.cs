using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Abstractions
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }

}
