using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Exceptions
{
    public sealed class CustomerNameInvalidException : DomainException
    {
        public CustomerNameInvalidException()
            : base("Customer name is invalid.")
        {
        }
    }
}
