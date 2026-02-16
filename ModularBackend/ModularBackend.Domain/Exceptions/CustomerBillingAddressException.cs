using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Exceptions
{
    public sealed class CustomerBillingAddressException : DomainException
    {
        public CustomerBillingAddressException()
            : base("Customer address is invalid. ")
        {
        }
    }
}
