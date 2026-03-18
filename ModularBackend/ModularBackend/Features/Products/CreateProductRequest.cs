using ModularBackend.Domain.Enumerables;

namespace ModularBackend.Api.Features.Products
{
    public sealed record CreateProductRequest(
        string Name,
        decimal BasePrice,
        decimal TaxRate,
        string Description,
        CurrencyType Currency,
        bool IsActive = false
    );
}
