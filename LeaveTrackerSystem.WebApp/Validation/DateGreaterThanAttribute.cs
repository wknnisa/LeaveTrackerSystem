using LeaveTrackerSystem.WebApp.Helpers;
using System.ComponentModel.DataAnnotations;

namespace LeaveTrackerSystem.WebApp.Validation
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
            {
                return new ValidationResult($"Unknown property: {_comparisonProperty}");
            }

            var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

            if (!currentValue.HasValue || !comparisonValue.HasValue)
            {
                return ValidationResult.Success;
            }

            if (currentValue.Value < comparisonValue.Value)
            {
                var httpContextAccessor = (IHttpContextAccessor?)validationContext.GetService(typeof(IHttpContextAccessor));

                string localizedMessage = "End Date must be on or after Start Date.";

                if (httpContextAccessor?.HttpContext != null)
                {
                    localizedMessage = LangHelper.Get(httpContextAccessor.HttpContext, "EndDateAfterStart");
                }

                return new ValidationResult(localizedMessage);
            }

            return ValidationResult.Success;
        }
    }
}
