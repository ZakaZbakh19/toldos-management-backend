using FluentValidation;
using ModularBackend.Application.Products.Commands.CreateProduct;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.UploadPhotoProducts
{
    public class UploadProductImageCommandValidator: AbstractValidator<UploadProductPhotosCommand>
    {
        private static readonly string[] AllowedContentTypes =
        [
            "image/jpeg",
                "image/png",
                "image/webp"
        ];

        public UploadProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product id is required.");

            RuleFor(x => x.Files)
                .NotNull()
                .WithMessage("Files is required.")
                .Must(files => files.Count > 0)
                .WithMessage("At least one file is required.")
                .Must(files => files.Count <= 5)
                .WithMessage("A maximum of 5 files is allowed.");

            RuleForEach(x => x.Files).ChildRules(file =>
            {
                file.RuleFor(f => f.FileName)
                    .NotEmpty()
                    .WithMessage("File name is required.");

                file.RuleFor(f => f.ContentType)
                    .NotEmpty()
                    .WithMessage("Content type is required.")
                    .Must(ct => AllowedContentTypes.Contains(ct))
                    .WithMessage("Only JPEG, PNG and WEBP images are allowed.");

                file.RuleFor(f => f.Length)
                    .GreaterThan(0)
                    .WithMessage("File cannot be empty.")
                    .LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Each file must be 5 MB or smaller.");

                file.RuleFor(f => f.Content)
                    .NotNull()
                    .WithMessage("File content is required.");
            });
        }
    }
}
