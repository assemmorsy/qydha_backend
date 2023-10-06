﻿using Qydha.Models;

namespace Qydha.Services;

public interface IAuthRepo
{
    Task<OperationResult<string>> LoginAsAnonymousAsync();
    Task<OperationResult<string>> RegisterAsync(UserRegisterDTO dto, string? userId);
    Task<OperationResult<string>> Login(UserLoginDto dto);
    Task<OperationResult<string>> ConfirmRegistrationWithPhone(string otpCode, string requestId);
}
