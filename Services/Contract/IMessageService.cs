using Qydha.Models;

namespace Qydha.Services;

public interface IMessageService
{
    Task<OperationResult<bool>> SendAsync(string phoneNum, string otp);
}
