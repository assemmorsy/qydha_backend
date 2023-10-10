using Qydha.Models;

namespace Qydha.Services;

public interface IFileService
{
    public Task<OperationResult<FileData>> UploadFile(string pathInBucket, IFormFile file);
    public Task<OperationResult<bool>> DeleteFile(string path);

}
