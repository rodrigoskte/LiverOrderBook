using FluentValidation.TestHelper;
using LiveOrderBook.Application.Validators;
using LiveOrderBook.Domain.Entities;
using Xunit;

namespace LiveOrderBook.Application.Test.Validators;

public class TradeMessageValidatorTest
{
    private readonly TradeMessageValidator _validator;

    public TradeMessageValidatorTest()
    {
        // Instância do validador
        _validator = new TradeMessageValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Null_Or_Zero()
    {
        // Arrange
        var tradeMessage = new TradeMessage
        {
            Price = 0,
            Asset = "ETH/USD",
            Quantity = 5
        };

        // Act
        var result = _validator.TestValidate(tradeMessage);

        // Assert
        result.ShouldHaveValidationErrorFor(tm => tm.Price)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_Asset_Is_Null_Or_Empty()
    {
        // Arrange
        var tradeMessage = new TradeMessage
        {
            Price = 1500,
            Asset = string.Empty,
            Quantity = 5
        };

        // Act
        var result = _validator.TestValidate(tradeMessage);

        // Assert
        result.ShouldHaveValidationErrorFor(tm => tm.Asset)
            .WithErrorMessage("Please enter the Asset.");
    }

    [Fact]
    public void Should_Have_Error_When_Quantity_Is_Null_Or_Zero()
    {
        // Arrange
        var tradeMessage = new TradeMessage
        {
            Price = 1500,
            Asset = "ETH/USD",
            Quantity = 0
        };

        // Act
        var result = _validator.TestValidate(tradeMessage);

        // Assert
        result.ShouldHaveValidationErrorFor(tm => tm.Quantity)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var tradeMessage = new TradeMessage
        {
            Price = 1500,
            Asset = "ETH/USD",
            Quantity = 5
        };

        // Act
        var result = _validator.TestValidate(tradeMessage);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}