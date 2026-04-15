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
                return query.OrderBy(p => p.Id);
            }

            return request.SortBy.ToLower() switch
            {
                "name" => request.Desc
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),

                "price" => request.Desc
                    ? query.OrderByDescending(p => p.BasePrice.Amount)
                    : query.OrderBy(p => p.BasePrice.Amount),

                _ => request.Desc
                    ? query.OrderByDescending(p => p.Id)
                    : query.OrderBy(p => p.Id)
            };
        }
    }
}
