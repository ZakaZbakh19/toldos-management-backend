using ModularBackend.Application.Abstractions.Common;
using System.Text.Json;

namespace ModularBackend.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static Task InsertPaginationMetadataAsync<T>(
                this HttpContext httpContext,
                PagedResult<T> pagedResult,
                CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(httpContext);
            ArgumentNullException.ThrowIfNull(pagedResult);

            var metadata = new
            {
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.PageSize,
                TotalPages = pagedResult.PageSize <= 0
                    ? 0
                    : (int)Math.Ceiling((double)pagedResult.TotalCount / pagedResult.PageSize),
                HasNextPage = pagedResult.Page * pagedResult.PageSize < pagedResult.TotalCount,
                HasPreviousPage = pagedResult.Page > 1
            };

            httpContext.Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

            return Task.CompletedTask;
        }
    }
}
