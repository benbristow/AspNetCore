using BenBristow.AspNetCore.DataValidationException.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shouldly;

namespace BenBristow.AspNetCore.DataValidationException.Tests.Exceptions;

public class DataValidationExceptionTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNoParameters_ShouldCreateExceptionWithDefaultMessage()
    {
        // Arrange & Act
        var exception = new DataValidationException.Exceptions.DataValidationException();

        // Assert
        exception.Message.ShouldBe("One or more data validation errors occurred.");
    }

    [Fact]
    public void Constructor_WithNullErrors_ShouldCreateExceptionWithEmptyErrors()
    {
        // Arrange & Act
        var exception = new DataValidationException.Exceptions.DataValidationException(errors: null);

        // Assert
        exception.Message.ShouldBe("One or more data validation errors occurred.");
    }

    [Fact]
    public void Constructor_WithErrors_ShouldCreateExceptionWithProvidedErrors()
    {
        // Arrange
        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "PropertyName", new List<string> { "Error message" } }
        };

        // Act
        var exception = new DataValidationException.Exceptions.DataValidationException(errors);

        // Assert
        exception.Message.ShouldBe("One or more data validation errors occurred.");
    }

    [Fact]
    public void Constructor_WithSingleErrorMessage_ShouldCreateExceptionWithError()
    {
        // Arrange
        const string errorMessage = "This is a single error";

        // Act
        var exception = new DataValidationException.Exceptions.DataValidationException(errorMessage);

        // Assert
        exception.Message.ShouldBe("One or more data validation errors occurred.");
    }

    #endregion

    #region AddToModelState Tests

    [Fact]
    public void AddToModelState_WithNoErrors_ShouldNotAddAnyModelErrors()
    {
        // Arrange
        var exception = new DataValidationException.Exceptions.DataValidationException();
        var modelState = new ModelStateDictionary();

        // Act
        exception.AddToModelState<TestModel>(modelState);

        // Assert
        modelState.IsValid.ShouldBeTrue();
        modelState.ErrorCount.ShouldBe(0);
    }

    [Fact]
    public void AddToModelState_WithSingleError_ShouldAddErrorToModelState()
    {
        // Arrange
        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "Name", new List<string> { "Name is required" } }
        };
        var exception = new DataValidationException.Exceptions.DataValidationException(errors);
        var modelState = new ModelStateDictionary();

        // Act
        exception.AddToModelState<TestModel>(modelState);

        // Assert
        modelState.IsValid.ShouldBeFalse();
        modelState.ErrorCount.ShouldBe(1);
        modelState["Name"]!.Errors[0].ErrorMessage.ShouldBe("Name is required");
    }

    [Fact]
    public void AddToModelState_WithMultipleErrors_ShouldAddAllErrorsToModelState()
    {
        // Arrange
        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "Name", new List<string> { "Name is required", "Name is too short" } },
            { "Email", new List<string> { "Email is invalid" } }
        };
        var exception = new DataValidationException.Exceptions.DataValidationException(errors);
        var modelState = new ModelStateDictionary();

        // Act
        exception.AddToModelState<TestModel>(modelState);

        // Assert
        modelState.IsValid.ShouldBeFalse();
        modelState.ErrorCount.ShouldBe(3);
        modelState["Name"]!.Errors.Count.ShouldBe(2);
        modelState["Email"]!.Errors.Count.ShouldBe(1);
    }

    [Fact]
    public void AddToModelState_WithMappedProperty_ShouldUsePropertyMapping()
    {
        // Arrange
        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "DomainName", new List<string> { "Name is required" } }
        };
        var exception = new DataValidationException.Exceptions.DataValidationException(errors);
        var modelState = new ModelStateDictionary();

        // Act
        exception.AddToModelState<TestModelWithMapping>(modelState);

        // Assert
        modelState.IsValid.ShouldBeFalse();
        modelState["MappedName"]!.Errors[0].ErrorMessage.ShouldBe("Name is required");
    }

    [Fact]
    public void AddToModelState_WithUnmappedProperty_ShouldUseOriginalPropertyName()
    {
        // Arrange
        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "UnmappedProperty", new List<string> { "Error message" } }
        };
        var exception = new DataValidationException.Exceptions.DataValidationException(errors);
        var modelState = new ModelStateDictionary();

        // Act
        exception.AddToModelState<TestModelWithMapping>(modelState);

        // Assert
        modelState.IsValid.ShouldBeFalse();
        modelState["UnmappedProperty"]!.Errors[0].ErrorMessage.ShouldBe("Error message");
    }

    [Fact]
    public void AddToModelState_WithEmptyKeyError_ShouldAddGeneralError()
    {
        // Arrange
        var exception = new DataValidationException.Exceptions.DataValidationException("General error message");
        var modelState = new ModelStateDictionary();

        // Act
        exception.AddToModelState<TestModel>(modelState);

        // Assert
        modelState.IsValid.ShouldBeFalse();
        modelState[""]!.Errors[0].ErrorMessage.ShouldBe("General error message");
    }

    #endregion

    #region Test Models

    private class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private class TestModelWithMapping
    {
        [DataValidationMapsToProperty("DomainName")]
        public string MappedName { get; set; } = string.Empty;
    }

    #endregion
}

