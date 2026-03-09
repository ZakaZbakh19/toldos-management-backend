using FluentValidation;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Domain.Entities;
using ModularBackend.Domain.Enumerables;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product(
                command.Name,
                new TaxRate(command.TaxRate),
                new Money(command.BasePrice, command.Currency),
                command.Description,
                command.IsActive
            );

            await _productRepository.AddAsync(product, cancellationToken);

            return product.Id;
        }
    }
}
