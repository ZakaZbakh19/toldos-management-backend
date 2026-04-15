using FluentValidation;
using ModularBackend.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModularBackend.Application.Features.Products.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");

            RuleFor(x => x.BasePrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Base price must be greater than or equal to zero.");

            RuleFor(x => x.Currency)
                .IsInEnum()
                .WithMessage("Currency is not valid.");

            RuleFor(x => x.TaxRate)
                .InclusiveBetween(0, 100)
                .WithMessage("Tax rate must be between 0 and 100.");
        }
    }

}
