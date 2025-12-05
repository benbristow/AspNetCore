using System;

namespace BenBristow.AspNetCore.DataValidationException.Attributes
{
    /// <summary>
    /// Attribute to map a property to a domain property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataValidationMapsToPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BenBristow.AspNetCore.DataValidationException.Attributes.DataValidationMapsToPropertyAttribute"/> class.
        /// </summary>
        /// <param name="domainPropertyName">The name of the domain property to map to.</param>
        public DataValidationMapsToPropertyAttribute(string domainPropertyName)
        {
            DomainPropertyName = domainPropertyName;
        }

        /// <summary>
        /// Gets the name of the domain property.
        /// </summary>
        public string DomainPropertyName { get; }
    }
}