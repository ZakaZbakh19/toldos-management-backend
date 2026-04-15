using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Users.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginRequestCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.email)
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
        }
    }
}
