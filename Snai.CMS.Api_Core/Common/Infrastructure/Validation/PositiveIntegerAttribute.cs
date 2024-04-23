using System.ComponentModel.DataAnnotations;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Validation
{
    public class PositiveIntegerAttribute : ValidationAttribute
    {
        private const string error = "须为数字";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (Validator.IsPositiveInteger(value as string))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage ?? error);
            }
        }
    }
}
