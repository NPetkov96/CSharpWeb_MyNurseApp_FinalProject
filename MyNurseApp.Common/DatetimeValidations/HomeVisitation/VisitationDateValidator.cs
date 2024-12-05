using System.ComponentModel.DataAnnotations;

namespace MyNurseApp.Common.DatetimeValidations.HomeVisitation
{
    public class VisitationDateValidator
    {
        public static ValidationResult? ValidateFutureDate(DateTime date, ValidationContext context)
        {
            if (date <= DateTime.Now)
            {
                return new ValidationResult("The date and time for visitation must be in the future.");
            }
            if (date > DateTime.Now.AddYears(1))
            {
                return new ValidationResult("The visit date must be within the next year.");
            }
            if (date.Hour < 8 || date.Hour > 18)
            {
                return new ValidationResult("Visiting hours must be between 8:00 AM and 6:00 PM.");
            }
            return ValidationResult.Success;
        }
    }
}
