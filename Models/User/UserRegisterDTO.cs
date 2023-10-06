using System.ComponentModel.DataAnnotations;
using Qydha.Helpers;

namespace Qydha.Models;

public class UserRegisterDTO
{
    [Required]
    [RegularExpression(InputValidators.Username.Pattern, ErrorMessage = InputValidators.Username.ErrorMessage)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [RegularExpression(InputValidators.Password.Pattern, ErrorMessage = InputValidators.Password.ErrorMessage)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression(InputValidators.Phone.Pattern, ErrorMessage = InputValidators.Phone.ErrorMessage)]
    public string Phone { get; set; } = string.Empty;

}
