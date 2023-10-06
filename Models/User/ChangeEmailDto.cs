using System.ComponentModel.DataAnnotations;
using Qydha.Helpers;

namespace Qydha.Models;

public class ChangeEmailDto
{
    [Required]
    [RegularExpression(InputValidators.Password.Pattern, ErrorMessage = InputValidators.Password.ErrorMessage)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [RegularExpression(InputValidators.Email.Pattern, ErrorMessage = InputValidators.Email.ErrorMessage)]
    public string NewEmail { get; set; } = string.Empty;

}
