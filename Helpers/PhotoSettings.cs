using Qydha.Models;

namespace Qydha.Helpers;

public class PhotoSettings
{

    public int MaxBytes { get; set; }

    public ICollection<string> AcceptedFileTypes { get; set; } = new List<string>();


    public bool IsValidSize(long size)
    {
        return size <= MaxBytes;
    }
    public bool IsValidMIME(string mime)
    {
        mime = mime.ToLower();
        return AcceptedFileTypes.Any(x => $".{x.ToLower()}".Equals(mime));
    }
    public OperationResult<bool> ValidateFile(IFormFile file)
    {
        var errors = new List<string>();

        if (file.Length == 0)
            errors.Add("File Can't be empty");
        if (!IsValidSize(file.Length))
            errors.Add("File size exceeded the limit");
        if (!IsValidMIME(Path.GetExtension(file.FileName)))
            errors.Add("File Type Not Accepted");

        if (errors.Count > 0)
        {
            return new OperationResult<bool>
            {
                Error = new()
                {
                    Code = ErrorCodes.InvalidPhotoFileInput,
                    Message = errors.Aggregate((acc, e) => acc + e)
                }
            };
        }
        return new OperationResult<bool> { Data = true, Message = "valid file" };
    }
}
