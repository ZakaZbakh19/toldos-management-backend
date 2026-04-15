using ModularBackend.Domain.Entities;

namespace ModularBackend.Application.Features.Products.GetProducts
{
    public sealed record ProductsDto(int totalItems, IEnumerable<Product> items);
}
