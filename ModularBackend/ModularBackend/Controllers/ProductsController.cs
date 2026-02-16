using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModularBackend.Api.Entities;

namespace ModularBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IReadOnlyCollection<Product> Get()
        {
            return new[]
            {
                new Product { Id = new Guid(), Name = "Product 1", Price = 10.0m },
                new Product { Id = new Guid(), Name = "Product 2", Price = 20.0m },
                new Product { Id = new Guid(), Name = "Product 3", Price = 30.0m }
            };
        }
    }
}
