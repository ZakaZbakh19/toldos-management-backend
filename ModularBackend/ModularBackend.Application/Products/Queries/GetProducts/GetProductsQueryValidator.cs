using FluentValidation;
using ModularBackend.Application.Users.Commands.Auth.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Queries.GetProducts
{
    public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
    {
        public GetProductsQueryValidator()
        {
            RuleFor(x => x.page)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.size)
                .GreaterThan(1);
        }
    }
}
