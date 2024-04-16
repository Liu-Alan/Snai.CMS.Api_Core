using System.ComponentModel.DataAnnotations;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Validation
{
    public class EnglishNumberCombinationAttribute: ValidationAttribute
    {
        private const string error = "英文字母加数字组合且6位及以上";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Validator.IsCombinationOfEnglishNumber(value as string, 6))
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
