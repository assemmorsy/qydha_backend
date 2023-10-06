namespace Qydha.Models;

public class Error
{
    public string Code { get; set; } = ErrorCodes.UnknownError;
    public string Message { get; set; } = string.Empty;

}
