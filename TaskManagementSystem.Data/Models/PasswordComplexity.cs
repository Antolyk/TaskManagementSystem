using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TaskManagementSystem.Data.Models
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;
            if (string.IsNullOrWhiteSpace(password))
                return new ValidationResult("Password is required.");

            if (password.Length < 3)
                return new ValidationResult("Password must be at least 3 characters long.");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return new ValidationResult("Password must contain at least one lowercase letter.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return new ValidationResult("Password must contain at least one uppercase letter.");

            if (!Regex.IsMatch(password, @"\d"))
                return new ValidationResult("Password must contain at least one digit.");

            if (!Regex.IsMatch(password, @"[@$!%*?&#_-]"))
                return new ValidationResult("Password must contain at least one special character.");

            return ValidationResult.Success!;
        }
    }

}
