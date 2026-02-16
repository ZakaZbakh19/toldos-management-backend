using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Abstractions
{
    public abstract class Entity
    {
        public Guid Id { get; private set; } = = Guid.NewGuid();
    }
}
