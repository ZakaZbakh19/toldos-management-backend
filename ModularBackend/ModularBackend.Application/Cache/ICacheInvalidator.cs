using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Cache
{
    public interface ICacheInvalidator
    {
        Task InvalidateProductsAsync(CancellationToken ct);
    }
}
