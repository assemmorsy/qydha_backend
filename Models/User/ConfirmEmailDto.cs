using System.ComponentModel.DataAnnotations;
using Qydha.Helpers;

namespace Qydha.Models;

public class ConfirmEmailDto
{
    [Required]
    [RegularExpression(InputValidators.OTPCode.Pattern, ErrorMessage = InputValidators.OTPCode.ErrorMessage)]

    public string Code { get; set; } = string.Empty;
    [Required]
    [RegularExpression(InputValidators.GuidId.Pattern, ErrorMessage = InputValidators.GuidId.ErrorMessage)]

    public string RequestId { get; set; } = string.Empty;
}
