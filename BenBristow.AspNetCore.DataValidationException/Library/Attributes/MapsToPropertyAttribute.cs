using System;

namespace BenBristow.AspNetCore.DataValidationException.Attributes
{
    /// <summary>
    /// Attribute to map a property to a domain property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MapsToPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToPropertyAttribute"/> class.
        /// </summary>
        /// <param name="domainPropertyName">The name of the domain property to map to.</param>
        public MapsToPropertyAttribute(string domainPropertyName)
        {
            DomainPropertyName = domainPropertyName;
        }

        /// <summary>
        /// Gets the name of the domain property.
        /// </summary>
        public string DomainPropertyName { get; }
    }
}