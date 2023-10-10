using System.ComponentModel.DataAnnotations;
using Qydha.Helpers;

namespace Qydha.Models;

public class DeleteUserDto
{
    [Required]
    [RegularExpression(InputValidators.Password.Pattern, ErrorMessage = InputValidators.Password.ErrorMessage)]
    public string Password { get; set; } = string.Empty;
}
