using FluentValidation;
namespace OSM.Application.Features.BaseSetup.MenuSetup.DeleteMenu
{
    public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
    {
        public DeleteMenuCommandValidator()
        {
            RuleFor(x => x.MenuId)
                .NotEmpty().WithMessage("Menu ID is required.")
                .MaximumLength(50).WithMessage("Menu ID must not exceed 50 characters.");
        }
    }
}
