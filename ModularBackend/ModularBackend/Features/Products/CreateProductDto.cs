using ModularBackend.Domain.Enumerables;

namespace ModularBackend.Api.Features.Products
{
    public sealed record CreateProductDTO(
        string Name,
        decimal BasePrice,
        decimal TaxRate,
        string Description,
        CurrencyType Currency,
        bool IsActive = false
    );
}
