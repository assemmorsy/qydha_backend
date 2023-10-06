using System.ComponentModel.DataAnnotations;
using Qydha.Helpers;

namespace Qydha.Models;

public class ChangePhoneDto
{
    [Required]
    [RegularExpression(InputValidators.Password.Pattern, ErrorMessage = InputValidators.Password.ErrorMessage)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [RegularExpression(InputValidators.Phone.Pattern, ErrorMessage = InputValidators.Phone.ErrorMessage)]
    public string NewPhone { get; set; } = string.Empty;
}
