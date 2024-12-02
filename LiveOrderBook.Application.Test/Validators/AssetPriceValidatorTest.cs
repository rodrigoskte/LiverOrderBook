using FluentValidation.TestHelper;
using LiveOrderBook.Application.Validators;
using LiveOrderBook.Domain.Entities;
using Xunit;

namespace LiveOrderBook.Application.Test.Validators;

public class AssetPriceValidatorTest
{
    private readonly AssetPriceValidator _validator;

    public AssetPriceValidatorTest()
    {
        // Instância do validador
        _validator = new AssetPriceValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Null_Or_Zero()
    {
        // Arrange
        var assetPrice = new AssetPrice
        {
            Price = 0,
            Asset = "BTC/USD",
            Quantity = 10
        };

        // Act
        var result = _validator.TestValidate(assetPrice);

        // Assert
        result.ShouldHaveValidationErrorFor(ap => ap.Price)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_Asset_Is_Null_Or_Empty()
    {
        // Arrange
        var assetPrice = new AssetPrice
        {
            Price = 50000,
            Asset = string.Empty,
            Quantity = 10
        };

        // Act
        var result = _validator.TestValidate(assetPrice);

        // Assert
        result.ShouldHaveValidationErrorFor(ap => ap.Asset)
            .WithErrorMessage("Please enter the Asset.");
    }

    [Fact]
    public void Should_Have_Error_When_Quantity_Is_Null_Or_Zero()
    {
        // Arrange
        var assetPrice = new AssetPrice
        {
            Price = 50000,
            Asset = "BTC/USD",
            Quantity = 0
        };

        // Act
        var result = _validator.TestValidate(assetPrice);

        // Assert
        result.ShouldHaveValidationErrorFor(ap => ap.Quantity)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var assetPrice = new AssetPrice
        {
            Price = 50000,
            Asset = "BTC/USD",
            Quantity = 10
        };

        // Act
        var result = _validator.TestValidate(assetPrice);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}