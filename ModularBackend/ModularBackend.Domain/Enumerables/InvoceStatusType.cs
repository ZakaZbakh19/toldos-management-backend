using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Enumerables
{
    public enum InvoceStatusType
    {
        Draft = 0,
        Issued = 1,
        Sent = 2,
        Paid = 3,
        Cancelled = 4
    }
}
