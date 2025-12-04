using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenBristow.AspNetCore.DataValidationException.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BenBristow.AspNetCore.DataValidationException.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when data validation fails, containing a collection of validation errors.
    /// </summary>
    public class DataValidationException : Exception
    {
        /// <summary>
        /// Gets the read-only dictionary of validation errors, keyed by property name.
        /// </summary>
        private IReadOnlyDictionary<string, IEnumerable<string>> Errors { get; } = new Dictionary<string, IEnumerable<string>>();
    
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class with optional validation errors.
        /// </summary>
        /// <param name="errors">A dictionary of validation errors, keyed by property name. If null, an empty dictionary is used.</param>
        public DataValidationException(Dictionary<string, IEnumerable<string>>? errors = null)
            : base("One or more data validation errors occurred.")
        {
            if (errors != null)
                Errors = errors;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class with a single error message.
        /// </summary>
        /// <param name="error">The error message to include.</param>
        public DataValidationException(string error)
            : this(new Dictionary<string, IEnumerable<string>> { { string.Empty, new List<string> { error } } })
        {
        }

        /// <summary>
        /// Adds the validation errors from this exception to the specified <see cref="ModelStateDictionary"/> for a given type,
        /// using property mappings defined by <see cref="MapsToPropertyAttribute"/> if available.
        /// </summary>
        /// <typeparam name="T">The type to which the errors are mapped.</typeparam>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> to add errors to.</param>
        public void AddToModelState<T>(ModelStateDictionary modelState) where T : class
        {
            var propertyMappings = BuildPropertyMappings<T>();

            foreach (var (propertyName, errors) in Errors)
            {
                var mappedPropertyName = propertyMappings.GetValueOrDefault(propertyName, propertyName);
                foreach (var error in errors)
                {
                    modelState.AddModelError(mappedPropertyName, error);
                }
            }
        }

        /// <summary>
        /// Builds a dictionary of property mappings from domain property names to actual property names
        /// based on <see cref="MapsToPropertyAttribute"/> attributes on the properties of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to inspect for property mappings.</typeparam>
        /// <returns>A dictionary where keys are domain property names and values are actual property names.</returns>
        private static Dictionary<string, string> BuildPropertyMappings<T>() where T : class
        {
            var mappings = new Dictionary<string, string>();
        
            foreach (var property in typeof(T).GetProperties())
            {
                var attribute = property.GetCustomAttribute<MapsToPropertyAttribute>();
                if (attribute != null)
                    mappings[attribute.DomainPropertyName] = property.Name;
            }

            return mappings;
        }
    }

    /// <summary>
    /// A factory class for building <see cref="DataValidationException"/> instances with validation errors for a specific type.
    /// </summary>
    /// <typeparam name="T">The type for which validation errors are being collected.</typeparam>
    public sealed class DataValidationExceptionFactory<T> where T : class
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
    
        /// <summary>
        /// Gets a value indicating whether any errors have been added to the factory.
        /// </summary>
        public bool HasErrors => _errors.Any();
    
        /// <summary>
        /// Adds a general error message not associated with a specific property.
        /// </summary>
        /// <param name="errorMessage">The error message to add.</param>
        /// <returns>The current <see cref="DataValidationExceptionFactory{T}"/> instance for chaining.</returns>
        public DataValidationExceptionFactory<T> AddError(string errorMessage)
        {
            if (!_errors.ContainsKey(string.Empty))
                _errors[string.Empty] = new List<string>();
            _errors[string.Empty].Add(errorMessage);
            return this;
        }
    
        /// <summary>
        /// Adds an error message associated with a specific property of type <typeparamref name="T"/>,
        /// identified by a property selector expression.
        /// </summary>
        /// <param name="propertySelector">An expression selecting the property to associate the error with.</param>
        /// <param name="errorMessage">The error message to add.</param>
        /// <returns>The current <see cref="DataValidationExceptionFactory{T}"/> instance for chaining.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="propertySelector"/> is not a simple member access.</exception>
        public DataValidationExceptionFactory<T> AddError(System.Linq.Expressions.Expression<Func<T, string>> propertySelector, string errorMessage)
        {
            if (!(propertySelector.Body is System.Linq.Expressions.MemberExpression member))
                throw new ArgumentException("propertySelector must be a simple member access",
                    nameof(propertySelector));
            
            var propertyName = member.Member.Name;
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();
            _errors[propertyName].Add(errorMessage);
            return this;
        }
    
        /// <summary>
        /// Creates a new <see cref="DataValidationException"/> instance with the collected errors.
        /// </summary>
        /// <returns>A new <see cref="DataValidationException"/> containing the errors.</returns>
        public DataValidationException Create()
        {
            var exception = new DataValidationException(_errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AsEnumerable()));
            return exception;
        }
    }
}