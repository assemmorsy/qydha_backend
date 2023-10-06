namespace Qydha.Models;

public class OperationResult<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public Error? Error { get; set; }

    public bool IsSuccess
    {
        get
        {
            return Error is null && Data is not null;
        }
    }
}
