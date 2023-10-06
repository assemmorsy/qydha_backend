using System.ComponentModel.DataAnnotations;
using Qydha.Helpers;

namespace Qydha.Models;

public class UserLoginDto
{
    [Required]
    [RegularExpression(InputValidators.Username.Pattern, ErrorMessage = InputValidators.Username.ErrorMessage)]

    public string Username { get; set; } = string.Empty;
    [Required]
    [RegularExpression(InputValidators.Password.Pattern, ErrorMessage = InputValidators.Password.ErrorMessage)]
    public string Password { get; set; } = string.Empty;

}
