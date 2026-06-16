using FluentValidation;

namespace OSM.Application.Features.BaseSetup.RoleSetup.CreateRole
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(256).WithMessage("Role name must not exceed 256 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(150).WithMessage("Description must not exceed 150 characters.");
        }
    }
}
