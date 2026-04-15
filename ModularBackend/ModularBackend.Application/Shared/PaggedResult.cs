using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Shared
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyCollection<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }
}
