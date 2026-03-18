using ModularBackend.Application.Products.Queries.GetProducts;
using ModularBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Queries.Extensions
{
    public static class ProductQueryExtensions
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
    }
}
