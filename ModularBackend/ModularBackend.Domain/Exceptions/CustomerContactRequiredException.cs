using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Exceptions
{
    public sealed class CustomerContactRequiredException : DomainException
    {
        public CustomerContactRequiredException()
            : base("Contact is required")
        {
        }
    }
}
