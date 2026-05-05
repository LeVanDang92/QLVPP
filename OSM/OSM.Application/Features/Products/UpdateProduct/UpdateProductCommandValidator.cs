using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM.Application.Features.Products.UpdateProduct
{
    public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x=>x.Id).NotEmpty().WithMessage("Product Id is required.");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Product name is required and cannot exceed 200 characters.");
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0.");
        }
    }
}
