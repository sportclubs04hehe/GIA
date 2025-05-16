using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Resolver
{
    /// <summary>
    /// Validates that a date property is not greater than another date property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        /// <summary>
        /// Initializes a new instance of the DateRangeValidationAttribute class.
        /// </summary>
        /// <param name="comparisonProperty">The property to compare with.</param>
        /// <param name="errorMessage">Optional custom error message.</param>
        public DateRangeValidationAttribute(string comparisonProperty, string? errorMessage = null)
        {
            _comparisonProperty = comparisonProperty;
            ErrorMessage = errorMessage ?? "Ngày bắt đầu không được lớn hơn ngày kết thúc";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var startDate = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException($"Property {_comparisonProperty} not found");

            var endDate = (DateTime)property.GetValue(validationContext.ObjectInstance)!;

            return startDate <= endDate
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}
