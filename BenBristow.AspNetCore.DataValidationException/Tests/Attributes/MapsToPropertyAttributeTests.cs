using BenBristow.AspNetCore.DataValidationException.Attributes;
using Shouldly;

namespace BenBristow.AspNetCore.DataValidationException.Tests.Attributes;

public class MapsToPropertyAttributeTests
{
    [Fact]
    public void Constructor_WithDomainPropertyName_ShouldSetProperty()
    {
        // Arrange
        const string domainPropertyName = "DomainName";

        // Act
        var attribute = new MapsToPropertyAttribute(domainPropertyName);

        // Assert
        attribute.DomainPropertyName.ShouldBe(domainPropertyName);
    }

    [Fact]
    public void Constructor_WithEmptyString_ShouldSetEmptyProperty()
    {
        // Arrange
        const string domainPropertyName = "";

        // Act
        var attribute = new MapsToPropertyAttribute(domainPropertyName);

        // Assert
        attribute.DomainPropertyName.ShouldBe(string.Empty);
    }

    [Fact]
    public void Attribute_ShouldBeApplicableToPropertiesOnly()
    {
        // Arrange & Act
        var attributeUsage = typeof(MapsToPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.ValidOn.ShouldBe(AttributeTargets.Property);
    }

    [Fact]
    public void Attribute_ShouldNotAllowMultiple()
    {
        // Arrange & Act
        var attributeUsage = typeof(MapsToPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();

        // Assert
        attributeUsage.ShouldNotBeNull();
        attributeUsage.AllowMultiple.ShouldBeFalse();
    }
}

