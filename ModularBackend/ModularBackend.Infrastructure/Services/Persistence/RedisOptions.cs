using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Services.Persistence
{
    public sealed class RedisOptions
    {
        public const string SectionName = "Redis";

        public string ConnectionString { get; init; } = string.Empty;
        public string InstanceName { get; init; } = "deco-llavaneres";
        public int ConversationTtlMinutes { get; init; } = 120;
    }
}
