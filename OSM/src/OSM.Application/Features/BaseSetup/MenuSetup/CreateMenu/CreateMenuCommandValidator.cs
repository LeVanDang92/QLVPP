using FluentValidation;

namespace OSM.Application.Features.BaseSetup.MenuSetup.CreateMenu
{
    public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
    {
        public CreateMenuCommandValidator()
        {
            RuleFor(x => x.MenuId)
                .NotEmpty().WithMessage("Menu ID is required.")
                .MaximumLength(50).WithMessage("Menu ID must not exceed 50 characters.");
            RuleFor(x => x.MenuName)
                .NotEmpty().WithMessage("Menu name is required.")
                .MaximumLength(200).WithMessage("Menu name must not exceed 200 characters.");
            RuleFor(x => x.MenuShortName)
                .MaximumLength(100).WithMessage("Menu short name must not exceed 100 characters.");
            RuleFor(x => x.MenuType)
                .NotEmpty().WithMessage("Menu type is required.")
                .MaximumLength(50).WithMessage("Menu type must not exceed 50 characters.");
            RuleFor(x => x.MenuGroup)
                .MaximumLength(100).WithMessage("Menu group must not exceed 100 characters.");
            RuleFor(x => x.MenuUrl)
                .MaximumLength(500).WithMessage("Menu URL must not exceed 500 characters.");
            RuleFor(x => x.ExternalUrl)
                .MaximumLength(1000).WithMessage("External URL must not exceed 1000 characters.");
            RuleFor(x => x.IconClass)
                .MaximumLength(100).WithMessage("Icon class must not exceed 100 characters.");
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative integer.");
            RuleFor(x => x.BadgeText)
                .MaximumLength(50).WithMessage("Badge text must not exceed 50 characters.");
            RuleFor(x => x.BadgeClass)
                .MaximumLength(200).WithMessage("Badge class must not exceed 200 characters.");
            RuleFor(x => x.ParentMenuId)
                .MaximumLength(50).WithMessage("Parent menu ID must not exceed 50 characters.");
        }
    }
}
