using FluentValidation;

namespace ModularBackend.Application.Products.Queries.GetProducts
{
    public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
    {
        private const int MaxPageSize = 100;

        public GetProductsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(MaxPageSize)
                .WithMessage($"PageSize must be between 1 and {MaxPageSize}.");

            RuleFor(x => x.Search)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Search));

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x)
                .Must(x =>
                    !x.MinPrice.HasValue ||
                    !x.MaxPrice.HasValue ||
                    x.MaxPrice >= x.MinPrice)
                .WithMessage("MaxPrice must be greater than or equal to MinPrice.");
        }
    }
}
