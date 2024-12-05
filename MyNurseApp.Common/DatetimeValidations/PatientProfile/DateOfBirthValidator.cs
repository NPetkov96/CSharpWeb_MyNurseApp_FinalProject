using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Common.DatetimeValidations.PatientProfile
{
    public class DateOfBirthValidator
    {
        public static ValidationResult? ValidateDateOfBirth(DateTime? dateOfBirth, ValidationContext context)
        {
            if (dateOfBirth == null)
            {
                return new ValidationResult("Date of birth is required.");
            }

            if (dateOfBirth > DateTime.Now)
            {
                return new ValidationResult("Date of birth cannot be in the future.");
            }

            DateTime minDate = new DateTime(1930, 1, 1);
            if (dateOfBirth < minDate)
            {
                return new ValidationResult("Date of birth cannot be earlier than January 1, 1930.");
            }

            int age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value.Date > DateTime.Now.AddYears(-age)) age--;

            if (age < 18)
            {
                return new ValidationResult("Patient must be at least 18 years old.");
            }

            return ValidationResult.Success;
        }
    }
}
