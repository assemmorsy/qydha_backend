using Qydha.Models;

namespace Qydha;

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
        return AcceptedFileTypes.Any(x => x.Equals(mime));
    }
    public OperationResult<IFormFile> ValidateFile(IFormFile file)
    {
        var errors = new List<string>();

        if (file.Length == 0)
            errors.Add("File Can't be empty");
        if (!IsValidSize(file.Length))
            errors.Add("File size exeeded the limit");
        if (!IsValidMIME(Path.GetExtension(file.FileName)))
            errors.Add("File Type Not Accepted");

        if (errors.Count > 0)
        {
            return new OperationResult<IFormFile>
            {
                Error = new()
                {
                    Code = ErrorCodes.InvalidPhotoFileInput,
                    Message = errors.Aggregate((acc, e) => acc + e)
                }
            };
        }
        return new OperationResult<IFormFile> { Data = file, Message = "valid file" };
    }
}
