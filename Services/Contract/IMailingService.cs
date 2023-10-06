using Qydha.Models;

namespace Qydha.Services;

public interface IMailingService
{
    Task<OperationResult<string>> SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments = null);
    Task<string> GenerateConfirmEmailBody(string otp, string requestId);
}
