using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.Common
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyCollection<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
    }
}
