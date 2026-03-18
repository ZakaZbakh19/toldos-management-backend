using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Entities
{
    public class ProductPhoto : Entity
    {
        public Guid ProductId { get; private set; }

        public string Url { get; private set; } = default!;
        public int Order { get; private set; }
        public bool IsMain { get; private set; }

        private ProductPhoto() { }

        public ProductPhoto(Guid productId, string url, int order, bool isMain = false)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId is required.", nameof(productId));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url is required.", nameof(url));

            if (order <= 0)
                throw new ArgumentOutOfRangeException("Order must be > 0.");

            ProductId = productId;
            Url = url;
            Order = order;
            IsMain = isMain;
        }
    }
}
