using ModularBackend.Domain.Entities;

namespace ModularBackend.Application.Products.Queries.GetProducts
{
    public sealed record ProductsDto(int totalItems, IEnumerable<Product> items);
}
