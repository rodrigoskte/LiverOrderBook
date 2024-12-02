using FluentValidation;
using LiveOrderBook.Domain.Entities;

namespace LiveOrderBook.Application.Validators;

public class TradeMessageValidator : AbstractValidator<TradeMessage>
{
    public TradeMessageValidator()
    {
        RuleFor(c => c.Price)
            .NotEmpty().WithMessage("Please enter the Price.")
            .NotNull().WithMessage("Please enter the Price.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        
        RuleFor(c => c.Asset)
            .NotEmpty().WithMessage("Please enter the Asset.")
            .NotNull().WithMessage("Please enter the Asset.");
        
        RuleFor(c => c.Quantity)
            .NotEmpty().WithMessage("Please enter the Quantity.")
            .NotNull().WithMessage("Please enter the Quantity.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}