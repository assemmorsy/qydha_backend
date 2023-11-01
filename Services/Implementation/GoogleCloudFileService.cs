using Google.Cloud.Storage.V1;
using Qydha.Models;

namespace Qydha.Services;

public class GoogleCloudFileService : IFileService
{
    private readonly StorageClient _client;
    private readonly string bucketName = "qydha_bucket";
    public GoogleCloudFileService()
    {
        _client = StorageClient.Create();
    }
    public async Task<OperationResult<bool>> DeleteFile(string path)
    {
        try
        {
            await _client.DeleteObjectAsync(bucketName, path);
            return new() { Data = true, Message = "File Deleted Successfully" };
        }
        catch (Exception e)
        {
            return new() { Error = new Error() { Code = ErrorCodes.FileDeleteError, Message = e.Message } };
        }
    }
    public async Task<OperationResult<FileData>> UploadFile(string pathInBucket, IFormFile file)
    {
        try
        {
            using (var ms = new MemoryStream())
            {
                string fileName = $"({Guid.NewGuid()})-{file.FileName}";
                string pathToFileInTheBucket = $"{pathInBucket}{fileName}";
                await file.CopyToAsync(ms);
                UploadObjectOptions options = new()
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead
                };
                var res = await _client.UploadObjectAsync(bucketName, pathToFileInTheBucket, file.ContentType, ms, options);
                string publicAccessLink = $"https://storage.googleapis.com/{bucketName}/{pathToFileInTheBucket}";

                return new() { Data = new() { Url = publicAccessLink, Path = pathToFileInTheBucket }, Message = "File Uploaded Successfully" };
            }
        }
        catch (Exception e)
        {
            return new() { Error = new Error() { Code = ErrorCodes.FileUploadError, Message = e.Message } };
        }
    }
}
