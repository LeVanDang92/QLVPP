using FluentValidation;

namespace OSM.Application.Features.BaseSetup.RoleSetup.UpdateRole
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Role ID is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(256).WithMessage("Role name must not exceed 256 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(150).WithMessage("Description must not exceed 150 characters.");
        }
    }
}
