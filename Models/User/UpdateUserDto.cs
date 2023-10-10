using System.ComponentModel.DataAnnotations;
using Qydha.Controllers.Attributes;
using Qydha.Helpers;

namespace Qydha.Models;
public class UpdateUserDto
{
    [Required]
    [RegularExpression(InputValidators.Name.Pattern, ErrorMessage = InputValidators.Name.ErrorMessage)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [BirthDate(ErrorMessage = "your age should be at least 7 years.")]
    public DateTime BirthDate { get; set; }
}