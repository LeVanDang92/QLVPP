using FluentValidation;

namespace OSM.Application.Features.BaseSetup.RoleMenuPermissions.UpdateRoleMenuPermissions;

public sealed class UpdateRoleMenuPermissionsCommandValidator : AbstractValidator<UpdateRoleMenuPermissionsCommand>
{
    public UpdateRoleMenuPermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotNull();

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.MenuId)
                .NotEmpty()
                .MaximumLength(50);
        });
    }
}
