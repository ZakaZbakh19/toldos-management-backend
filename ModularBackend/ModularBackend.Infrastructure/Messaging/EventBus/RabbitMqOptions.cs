using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Messaging.EventBus
{
    public sealed class RabbitMqOptions
    {
        public const string SectionName = "RabbitMq";

        public string HostName { get; set; } = default!;
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ExchangeName { get; set; } = "main-exchange";
    }
}
