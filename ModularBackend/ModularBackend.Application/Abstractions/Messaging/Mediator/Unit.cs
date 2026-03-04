using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Messaging.Mediator
{
    public struct Unit
    {
        public static readonly Unit Instance = new Unit();
    }
}
