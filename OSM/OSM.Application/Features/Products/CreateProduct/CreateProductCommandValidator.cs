using FluentValidation;

namespace OSM.Application.Features.Products.CreateProduct
{
    public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        /// <summary>
        /// Check dữ liệu đầu vào.
        /// Không để request xấu vào handler.
        /// </summary>
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("StockQuantity must be greater than or equal to 0.");
        }
    }
}
