using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Events.IntegrationEvents
{
    public static class IntegrationEventNames
    {
        public static HashSet<string> AllEventNames { get; } = new HashSet<string>
        {
            ProductCreatedV1
        };

        public const string ProductCreatedV1 = "catalog.product.created.v1";
    }
}
