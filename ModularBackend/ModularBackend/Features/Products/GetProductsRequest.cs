namespace ModularBackend.Api.Features.Products
{
    public sealed class GetProductsRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public string? Search { get; init; }
        public bool? IsActive { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }

        public string? SortBy { get; init; }
        public bool Desc { get; init; }
    }
}
