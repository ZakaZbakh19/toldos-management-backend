using ModularBackend.Application.Features.Products.GetProducts;
using ModularBackend.Domain.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace ModularBackend.Infrastructure.Persistence.Queries.Extensions
{
    public static class ProductsQueryExtensions
    {
        public static IQueryable<Product> ApplyFilters(
            this IQueryable<Product> query,
            GetProductsQuery request)
        {
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(p => p.Name.Contains(request.Search));

            if (request.IsActive.HasValue)
                query = query.Where(p => p.IsActive == request.IsActive.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.BasePrice.Amount >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.BasePrice.Amount <= request.MaxPrice.Value);

            return query;
        }

        public static IQueryable<Product> ApplySorting(
            this IQueryable<Product> query,
            GetProductsQuery request)
        {
            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                return query.OrderBy(p => p.Id); // Orden por defecto
            }

            // Definimos la expresión de la clave según el string
            Expression<Func<Product, object>> keySelector = request.SortBy.ToLower() switch
            {
                "name" => p => p.Name,
                "price" => p => p.BasePrice,
                _ => p => p.Id 
            };

            return request.Desc
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }
    }
}
