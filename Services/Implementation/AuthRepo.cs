
using System.Security.Claims;
using Qydha.Entities;
using Qydha.Models;




namespace Qydha.Services;

public class AuthRepo : IAuthRepo
{
    private readonly TokenManager _tokenManager;
    private readonly IUserRepo _userRepo;
    private readonly ISMSService _smsService;
    private readonly RegistrationOTPRequestRepo _registrationOTPRequestRepo;
    private readonly OtpManager _otpManager;
    public AuthRepo(TokenManager tokenManager, IUserRepo userRepo, OtpManager otpManager, ISMSService smsService, RegistrationOTPRequestRepo registrationOTPRequestRepo)
    {
        _tokenManager = tokenManager;
        _userRepo = userRepo;
        _smsService = smsService;
        _registrationOTPRequestRepo = registrationOTPRequestRepo;
        _otpManager = otpManager;
    }


    public async Task<OperationResult<string>> LoginAsAnonymousAsync()
    {
        var anonymousUser = new User() { Created_On = DateTime.Now, Last_Login = DateTime.Now, Is_Anonymous = true };
        var registerAnonUserRes = await _userRepo.AddUser(anonymousUser);
        if (!registerAnonUserRes.IsSuccess)
            return new() { Error = new() { Code = ErrorCodes.UnknownError, Message = "Unknown Error" } };
        anonymousUser = registerAnonUserRes.Data;
        var claims = new List<Claim>()
        {
            new Claim("sub", anonymousUser!.Id.ToString()),
            new Claim("userId", anonymousUser!.Id.ToString()),
            new Claim("isAnonymous", anonymousUser.Is_Anonymous.ToString())
        };
        return new()
        {
            Data = _tokenManager.Generate(claims),
            Message = "Anonymous User logged In Successfully"
        };
    }

    public async Task<OperationResult<string>> ConfirmRegistrationWithPhone(string otpCode, string requestId)
    {
        if (!_otpManager.VerifyOTP(otpCode))
            return new()
            {
                Error = new() { Code = ErrorCodes.OTPExceededTimeLimit, Message = "OTP Exceed Time Limit" }
            };

        var otp_request = await _registrationOTPRequestRepo.FindAsync(requestId);
        if (otp_request is null)
            return new()
            {
                Error = new() { Code = ErrorCodes.RegistrationRequestNotFound, Message = "Registration Request Not Found." }
            };
        if (otp_request.OTP != otpCode)
            return new()
            {
                Error = new() { Code = ErrorCodes.InvalidOTP, Message = "Invalid OTP." }
            };


        var saveUserRes = await _userRepo.SaveUserFromRegistrationOTPRequest(otp_request);
        if (!saveUserRes.IsSuccess) return new() { Error = saveUserRes.Error };
        var user = saveUserRes.Data;
        var claims = new List<Claim>()
            {
                new Claim("sub", user!.Id.ToString()),
                new Claim("userId", user!.Id.ToString()),
                new Claim("username", user.Username! ),
                new Claim("phone", user.Phone! ),
                new Claim("isAnonymous", user.Is_Anonymous.ToString()),
            };
        var jwtToken = _tokenManager.Generate(claims);
        return new()
        {
            Data = jwtToken,
            Message = "User Registered Successfully"
        };
    }

    public async Task<OperationResult<string>> Login(UserLoginDto dto)
    {
        var findUserRes = await _userRepo.FindUserByUsername(dto.Username);
        var user = findUserRes.Data;
        if (!findUserRes.IsSuccess || !BCrypt.Net.BCrypt.Verify(dto.Password, user!.Password_Hash))
            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.InvalidCredentials,
                    Message = "Invalid Credentials."
                }
            };

        if (!await _userRepo.UpdateUserLastLoginToNow(user.Id.ToString()))
            return new()
            {
                Error = new() { Code = ErrorCodes.UserNotFound, Message = "User not found to update last login " }
            };

        var claims = new List<Claim>()
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("username", user.Username ?? ""),
                new Claim("phone", user.Phone ?? ""),
                new Claim("isAnonymous", user.Is_Anonymous.ToString()),
            };
        var jwtToken = _tokenManager.Generate(claims);
        return new OperationResult<string>()
        {
            Data = jwtToken,
            Message = "User Logged In Successfully."
        };
    }

    public async Task<OperationResult<string>> RegisterAsync(UserRegisterDTO dto, string? userId)
    {
        if (await _userRepo.IsUsernameUsed(dto.Username))
            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.DbUniqueViolation,
                    Message = "Username is already used."
                }
            };
        if (await _userRepo.IsPhoneUsed(dto.Phone))
            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.DbUniqueViolation,
                    Message = "phone number is already used."
                }
            };

        // Compute OTP
        var otp = _otpManager.GenerateOTP();

        var result = await _smsService.SendAsync(dto.Phone, $" رمز التحقق لتطبيق قيدها : {otp}");

        if (!string.IsNullOrEmpty(result.ErrorMessage))
            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.OTPSendingError,
                    Message = $"OTP Error Code :: {result.ErrorCode} => message :: {result.ErrorMessage}"
                }
            };

        // SAVE REQUEST DATA 
        var otp_request = await _registrationOTPRequestRepo.AddAsync(new RegistrationOTPRequest()
        {
            Username = dto.Username,
            Phone = dto.Phone,
            Password_Hash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            OTP = otp,
            User_Id = userId is not null ? Guid.Parse(userId) : null
        });
        // RETURN SUCCESS
        return new()
        {
            Data = otp_request.Id.ToString(),
            Message = "OTP sent Successfully"
        };
    }

}
