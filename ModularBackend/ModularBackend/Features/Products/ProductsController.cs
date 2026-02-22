using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Products.Commands.CreateProduct;

namespace ModularBackend.Api.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO dto, CancellationToken ct)
        {
            var command = new CreateProductCommand(
                dto.Name,
                dto.BasePrice,
                dto.TaxRate,
                dto.Description,
                dto.Currency,
                dto.IsActive
            );

            var id = await _mediator.Send(command, ct);

            return CreatedAtAction(nameof(Application.Products.Queries.GetProductById), new { id }, null);
        }
    }
}
