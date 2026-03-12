using FluentValidation;

namespace ModularBackend.Application.Users.Commands.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterRequestCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
        }
    }
}
