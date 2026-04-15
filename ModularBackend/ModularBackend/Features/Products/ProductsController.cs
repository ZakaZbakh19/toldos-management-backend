using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using ModularBackend.Application.Features.Products.Common;
using ModularBackend.Application.Features.Products.CreateProduct;
using ModularBackend.Application.Features.Products.GetProductById;
using ModularBackend.Application.Features.Products.GetProducts;
using ModularBackend.Application.Features.Products.UploadPhotoProducts;
using ModularBackend.Application.Mediator;
using ModularBackend.Application.Shared;

namespace ModularBackend.Api.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [OutputCache(PolicyName = "ProductById")]
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
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ProductManager")]
        [OutputCache(Tags = ["products"])]
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
        [OutputCache(Tags = ["products"])]
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
