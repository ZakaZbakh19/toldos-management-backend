using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Products.Commands.CreateProduct;
using ModularBackend.Application.Products.Queries.GetProductById;
using ModularBackend.Domain.Entities;

namespace ModularBackend.Api.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ProductManager")]
        [EnableRateLimiting("authenticated-user-standard")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetProductByIdQuery(id), ct);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Policy = "ProductManager")]
        [EnableRateLimiting("authenticated-user-standard")]
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

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
    }
}
