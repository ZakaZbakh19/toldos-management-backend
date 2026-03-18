using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ModularBackend.Application.Abstractions.Common;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Products.Commands.CreateProduct;
using ModularBackend.Application.Products.Commands.UploadPhotoProducts;
using ModularBackend.Application.Products.Queries.Common;
using ModularBackend.Application.Products.Queries.GetProductById;
using ModularBackend.Application.Products.Queries.GetProducts;
using ModularBackend.Domain.Entities;

namespace ModularBackend.Api.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [ProducesResponseType<ProductDetailDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetProductByIdQuery(id), ct);
            return Ok(dto);
        }

        [HttpGet]
        [Authorize(Policy = "ProductManager")]
        [EnableRateLimiting("authenticated-user-standard")]
        [ProducesResponseType<PagedResult<ProductDetailDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest dto, CancellationToken ct)
        {
            var query = new GetProductsQuery(
                dto.Page,
                dto.PageSize,
                dto.Search,
                dto.IsActive,
                dto.MinPrice,
                dto.MaxPrice,
                dto.SortBy,
                dto.Desc
            );

            var result = await _mediator.Send(query, ct);
            //await HttpContext.InsertPaginationMetadataAsync(result, ct);
            return Ok(result.Items);
        }

        [HttpPost]
        [Authorize(Policy = "ProductManager")]
        [EnableRateLimiting("authenticated-user-standard")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest dto, CancellationToken ct)
        {
            var command = new CreateProductCommand(
                dto.Name,
                dto.BasePrice,
                dto.TaxRate,
                dto.Description,
                dto.Currency,
                dto.IsActive
            );

            var product = await _mediator.Send(command, ct);

            return CreatedAtAction(nameof(GetById), new { product.Id }, product);
        }

        [HttpPost("{productId:guid}/photos")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "ProductManager")]
        [EnableRateLimiting("authenticated-user-standard")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadPhotos(
            [FromRoute] Guid productId,
            [FromForm] UploadProductPhotosRequest request,
            CancellationToken ct)
        {
            var fileInputs = new List<UploadPhotoInput>();

            foreach (var file in request.Files)
            {
                var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, ct);
                memoryStream.Position = 0;

                fileInputs.Add(new UploadPhotoInput(
                    file.FileName,
                    file.ContentType,
                    file.Length,
                    memoryStream));
            }

            var command = new UploadProductPhotosCommand(productId, fileInputs);

            var result = await _mediator.Send(command, ct);

            return Ok(result);
        }
    }
}
