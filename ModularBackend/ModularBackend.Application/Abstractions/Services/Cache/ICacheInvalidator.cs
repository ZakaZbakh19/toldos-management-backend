using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Cache
{
    public interface ICacheInvalidator
    {
        Task InvalidateProductsAsync(CancellationToken ct);
    }
}
