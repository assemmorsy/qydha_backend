using System.ComponentModel.DataAnnotations;
using Qydha.Helpers;

namespace Qydha.Models;

public class ChangeUsernameDto
{

    [Required]
    [RegularExpression(InputValidators.Password.Pattern, ErrorMessage = InputValidators.Password.ErrorMessage)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [RegularExpression(InputValidators.Username.Pattern, ErrorMessage = InputValidators.Username.ErrorMessage)]
    public string NewUsername { get; set; } = string.Empty;

}
