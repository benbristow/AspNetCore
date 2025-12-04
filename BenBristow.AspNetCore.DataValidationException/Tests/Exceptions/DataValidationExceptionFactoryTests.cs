using BenBristow.AspNetCore.DataValidationException.Exceptions;
using Shouldly;

namespace BenBristow.AspNetCore.DataValidationException.Tests.Exceptions;

public class DataValidationExceptionFactoryTests
{
    #region AddError (General) Tests

    [Fact]
    public void AddError_WithSingleGeneralError_ShouldCreateExceptionWithError()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var exception = factory
            .AddError("General error message")
            .Create();

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe("One or more data validation errors occurred.");
    }

    [Fact]
    public void AddError_WithMultipleGeneralErrors_ShouldCreateExceptionWithAllErrors()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var exception = factory
            .AddError("First error")
            .AddError("Second error")
            .Create();

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void AddError_ShouldReturnFactoryForChaining()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var result = factory.AddError("Error message");

        // Assert
        result.ShouldBeSameAs(factory);
    }

    #endregion

    #region AddError (Property) Tests

    [Fact]
    public void AddError_WithPropertySelector_ShouldCreateExceptionWithPropertyError()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var exception = factory
            .AddError(x => x.Name, "Name is required")
            .Create();

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void AddError_WithMultiplePropertyErrors_ShouldCreateExceptionWithAllErrors()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var exception = factory
            .AddError(x => x.Name, "Name is required")
            .AddError(x => x.Name, "Name is too short")
            .AddError(x => x.Email, "Email is invalid")
            .Create();

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void AddError_WithPropertySelector_ShouldReturnFactoryForChaining()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var result = factory.AddError(x => x.Name, "Error message");

        // Assert
        result.ShouldBeSameAs(factory);
    }

    [Fact]
    public void AddError_WithInvalidPropertySelector_ShouldThrowArgumentException()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            factory.AddError(x => x.Name.ToUpper(), "Error message"));
    }

    [Fact]
    public void AddError_WithInvalidPropertySelector_ShouldHaveCorrectParameterName()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var exception = Should.Throw<ArgumentException>(() =>
            factory.AddError(x => x.Name.ToUpper(), "Error message"));

        // Assert
        exception.ParamName.ShouldBe("propertySelector");
    }

    #endregion

    #region Mixed Errors Tests

    [Fact]
    public void Create_WithMixedErrors_ShouldCreateExceptionWithAllErrors()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var exception = factory
            .AddError("General error")
            .AddError(x => x.Name, "Name error")
            .AddError(x => x.Email, "Email error")
            .Create();

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void Create_WithNoErrors_ShouldCreateEmptyException()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        var exception = factory.Create();

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe("One or more data validation errors occurred.");
    }

    #endregion

    #region HasErrors Tests

    [Fact]
    public void HasErrors_WhenNoErrorsAdded_ShouldReturnFalse()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act & Assert
        factory.HasErrors.ShouldBeFalse();
    }

    [Fact]
    public void HasErrors_WhenGeneralErrorAdded_ShouldReturnTrue()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        factory.AddError("General error");

        // Assert
        factory.HasErrors.ShouldBeTrue();
    }

    [Fact]
    public void HasErrors_WhenPropertyErrorAdded_ShouldReturnTrue()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        factory.AddError(x => x.Name, "Name error");

        // Assert
        factory.HasErrors.ShouldBeTrue();
    }

    [Fact]
    public void HasErrors_WhenMultipleErrorsAdded_ShouldReturnTrue()
    {
        // Arrange
        var factory = new DataValidationExceptionFactory<TestModel>();

        // Act
        factory.AddError("General error")
               .AddError(x => x.Name, "Name error")
               .AddError(x => x.Email, "Email error");

        // Assert
        factory.HasErrors.ShouldBeTrue();
    }

    #endregion

    #region Test Models

    private class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    #endregion
}

