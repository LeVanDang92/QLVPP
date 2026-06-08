using FluentValidation;

namespace OSM.Application.Features.Auth.Register
{
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Role)
              .NotEmpty();

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(x => x.PasswordShow)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
