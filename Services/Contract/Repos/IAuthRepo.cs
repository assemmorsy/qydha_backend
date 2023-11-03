using Qydha.Models;

namespace Qydha.Services;

public interface IAuthRepo
{
    Task<OperationResult<string>> LoginAsAnonymousAsync(UserLoginAnonymousDto userLoginAnonymousDto);
    Task<OperationResult<string>> RegisterAsync(UserRegisterDTO dto, string? userId);
    Task<OperationResult<TokenWithUserDataDto>> Login(UserLoginDto dto);
    Task<OperationResult<TokenWithUserDataDto>> ConfirmRegistrationWithPhone(string otpCode, string requestId);
    Task<OperationResult<bool>> Logout(Guid userId);
}
