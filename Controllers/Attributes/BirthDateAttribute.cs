using System.ComponentModel.DataAnnotations;
namespace Qydha.Controllers.Attributes;

public class BirthDateAttribute : ValidationAttribute
{

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is not null && value is DateTime birthDate)
        {
            // Calculate the date 7 years ago from now
            DateTime minAge = DateTime.Today.AddYears(-7);
            DateTime maxAge = DateTime.Today.AddYears(-150);
            
            // Check if the birthdate is earlier than ten years ago
            if (birthDate < minAge  && birthDate > maxAge)
            {
                return true;
            }
        }
        return false;

    }
}
