using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.OutputCaching;
using ModularBackend.Application.Cache;

namespace ModularBackend.Infrastructure.Cache
{
    public sealed class OutputCacheInvalidator : ICacheInvalidator
    {
        private readonly IOutputCacheStore _store;

        public OutputCacheInvalidator(IOutputCacheStore store)
        {
            _store = store;
        }

        public async Task InvalidateProductsAsync(CancellationToken ct)
            => await _store.EvictByTagAsync("products", ct);
    }
}
