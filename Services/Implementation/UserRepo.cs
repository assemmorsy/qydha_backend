using System.Data;
using Dapper;
using Qydha.Entities;
using Qydha.Models;

namespace Qydha.Services;

public class UserRepo : IUserRepo
{
    private readonly ISMSService _smsService;
    private readonly UpdatePhoneOTPRequestRepo _updatePhoneOTPRequestRepo;
    private readonly UpdateEmailRequestRepo _updateEmailRequestRepo;

    private readonly IDbConnection _dbConnection;
    private readonly OtpManager _otpManager;
    private readonly IMailingService _mailingService;

    public UserRepo(IDbConnection dbConnection, ISMSService smsService, IMailingService mailingService, OtpManager otpManager, UpdatePhoneOTPRequestRepo updatePhoneOTPRequestRepo, UpdateEmailRequestRepo updateEmailRequestRepo)
    {
        _updatePhoneOTPRequestRepo = updatePhoneOTPRequestRepo;
        _dbConnection = dbConnection;
        _smsService = smsService;
        _otpManager = otpManager;
        _mailingService = mailingService;
        _updateEmailRequestRepo = updateEmailRequestRepo;
    }

    public async Task<OperationResult<User>> AddUser(User user)
    {
        var sql = @"INSERT INTO 
                    USERS ( username, name, password_hash, phone, email, is_anonymous, birth_date, created_on, last_login, avatar_url, avatar_path , is_phone_confirmed , is_email_confirmed ) 
                    VALUES ( @Username , @Name , @Password_Hash, @Phone , @Email ,@Is_Anonymous , @Birth_Date , @Created_On , @Last_Login , @Avatar_Url ,@Avatar_Path ,
                    @Is_Phone_Confirmed , @Is_Email_Confirmed )
                    RETURNING Id;";
        var userId = await _dbConnection.QuerySingleAsync<Guid>(sql, user);
        user.Id = userId;
        return new OperationResult<User>()
        {
            Data = user,
            Message = "User Added Successfully"
        };
    }
    public async Task<OperationResult<User>> UpdateUser(User user)
    {
        var sql = @"UPDATE USERS 
                    SET username = @Username,
                        name  = @Name ,
                        password_hash = @Password_Hash ,
                        phone = @Phone , 
                        email = @Email ,
                        is_anonymous = @Is_Anonymous ,
                        birth_date = @Birth_Date ,
                        last_login = @Last_Login ,
                        is_phone_confirmed = @Is_Phone_Confirmed , 
                        is_email_confirmed = @Is_Email_Confirmed,
                        avatar_url = @Avatar_Url, 
                        avatar_path = @Avatar_Path
                    WHERE id = @Id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, user);
        if (effectedRows != 1) return new OperationResult<User>() { Error = new Error() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new OperationResult<User>()
        {
            Data = user,
            Message = "User Updated Successfully"
        };
    }
    public async Task<OperationResult<User>> FindUserById(string userId)
    {
        var sql = "select *  from Users where id = @userId;";
        User? user = await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { userId = Guid.Parse(userId) });
        if (user is null) return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new()
        {
            Data = user,
            Message = "User Fetched Successfully"
        };
    }
    public async Task<bool> UpdateUserLastLoginToNow(string userId)
    {
        return await UpdateUserPropertyById(userId, "last_login", DateTime.UtcNow);
    }
    public async Task<bool> UpdateUserPropertyById<T>(string userId, string propName, T newValue)
    {
        var sql = @$"UPDATE USERS 
                        SET {propName} = @newValue 
                    where id = @userId;";

        var effectedRows = await _dbConnection.ExecuteAsync(sql, new { userId = Guid.Parse(userId), newValue });
        return effectedRows == 1;
    }
    public async Task<OperationResult<User>> FindUserByUsername(string username)
    {
        var sql = "select  *  from Users where username = @username;";
        User? user = await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { username });
        if (user is null) return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new()
        {
            Data = user,
            Message = "User Fetched Successfully"
        };
    }
    public async Task<OperationResult<User>> FindUserByPhone(string phone)
    {
        var sql = "select *  from Users where phone = @phone;";
        User? user = await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { phone });
        if (user is null) return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new()
        {
            Data = user,
            Message = "User Fetched Successfully"
        };
    }
    public async Task<OperationResult<User>> FindUserByEmail(string email)
    {
        var sql = "select *  from Users where email = @email;";
        User? user = await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { email });
        if (user is null) return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new()
        {
            Data = user,
            Message = "User Fetched Successfully"
        };
    }
    public async Task<bool> IsUsernameUsed(string username)
    {
        var opRes = await FindUserByUsername(username);
        return opRes.IsSuccess && opRes.Data is not null;
    }
    public async Task<bool> IsPhoneUsed(string phone)
    {
        var opRes = await FindUserByPhone(phone);
        return opRes.IsSuccess && opRes.Data is not null;
    }
    public async Task<bool> IsEmailUsed(string email)
    {
        var opRes = await FindUserByEmail(email);
        return opRes.IsSuccess && opRes.Data is not null;
    }
    public async Task<bool> UpdateUserAvatar(string userId, string avatar_url, string avatar_path)
    {
        var sql = @"UPDATE USERS 
                    SET avatar_url = @avatar_url , 
                        avatar_path = @avatar_path 
                    WHERE id = @userId;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, new { userId, avatar_url, avatar_path });
        return effectedRows == 1;
    }
    public async Task<OperationResult<User>> SaveUserFromRegistrationOTPRequest(RegistrationOTPRequest otpRequest)
    {
        if (otpRequest.User_Id is not null)
        {
            var findUserRes = await FindUserById(otpRequest.User_Id.Value.ToString());
            if (!findUserRes.IsSuccess)
            {
                findUserRes.Error!.Code = ErrorCodes.AnonymousUserNotFound;
                findUserRes.Error!.Message = "Anonymous User Not Found";
                return findUserRes;
            }

            findUserRes.Data!.Username = otpRequest.Username;
            findUserRes.Data!.Phone = otpRequest.Phone;
            findUserRes.Data!.Last_Login = DateTime.UtcNow;
            findUserRes.Data!.Is_Phone_Confirmed = true;
            findUserRes.Data!.Password_Hash = otpRequest.Password_Hash;
            findUserRes.Data!.Is_Anonymous = false;

            return await UpdateUser(findUserRes.Data);
        }
        else
        {
            return await AddUser(new User()
            {
                Username = otpRequest.Username,
                Password_Hash = otpRequest.Password_Hash,
                Phone = otpRequest.Phone,
                Created_On = DateTime.UtcNow,
                Last_Login = DateTime.UtcNow,
                Is_Phone_Confirmed = true,
                Is_Anonymous = false
            });
        }
    }
    public async Task<OperationResult<bool>> UpdateUserPassword(string userId, string oldPassword, string newPassword)
    {
        var findUserRes = await FindUserById(userId);
        if (!findUserRes.IsSuccess) return new() { Error = findUserRes.Error };
        if (!BCrypt.Net.BCrypt.Verify(oldPassword, findUserRes.Data!.Password_Hash))
            return new() { Error = new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" } };
        var updateSuccess = await UpdateUserPropertyById(userId, "password_hash", BCrypt.Net.BCrypt.HashPassword(newPassword));
        if (!updateSuccess)
            return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new() { Data = true, Message = "Password updated successfully" };
    }
    public async Task<OperationResult<bool>> UpdateUserUsername(string userId, string password, string newUsername)
    {
        var findUserByUsernameRes = await FindUserByUsername(newUsername);
        if (findUserByUsernameRes.IsSuccess && findUserByUsernameRes.Data!.Id.ToString() != userId)
            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.DbUniqueViolation,
                    Message = "Username is already used."
                }
            };
        var findUserRes = await FindUserById(userId);
        if (!findUserRes.IsSuccess) return new() { Error = findUserRes.Error };
        if (!BCrypt.Net.BCrypt.Verify(password, findUserRes.Data!.Password_Hash))
            return new() { Error = new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" } };
        var updateSuccess = await UpdateUserPropertyById(userId, "username", newUsername);
        if (!updateSuccess)
            return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new() { Data = true, Message = "Username updated successfully" };
    }
    public async Task<OperationResult<string>> UpdateUserPhone(string userId, string password, string newPhone)
    {
        var findUserByPhoneRes = await FindUserByPhone(newPhone);
        if (findUserByPhoneRes.IsSuccess)
            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.DbUniqueViolation,
                    Message = "Phone is already used."
                }
            };
        var findUserRes = await FindUserById(userId);
        if (!findUserRes.IsSuccess) return new() { Error = findUserRes.Error };

        if (!BCrypt.Net.BCrypt.Verify(password, findUserRes.Data!.Password_Hash))
            return new() { Error = new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" } };

        // Compute OTP
        var otp = _otpManager.GenerateOTP();

        var result = await _smsService.SendAsync(newPhone, $" رمز التحقق لتطبيق قيدها : {otp}");

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
        var otp_request = await _updatePhoneOTPRequestRepo.AddAsync(new()
        {
            Phone = newPhone,
            OTP = otp,
            User_Id = Guid.Parse(userId),
            Created_On = DateTime.UtcNow
        });
        // RETURN SUCCESS
        return new()
        {
            Data = otp_request.Id.ToString(),
            Message = "OTP sent Successfully"
        };


    }
    public async Task<OperationResult<bool>> ConfirmPhoneUpdate(string userId, string code, string requestId)
    {
        if (!_otpManager.VerifyOTP(code))
            return new()
            {
                Error = new() { Code = ErrorCodes.OTPExceededTimeLimit, Message = "OTP Exceed Time Limit" }
            };

        var otp_request = await _updatePhoneOTPRequestRepo.FindAsync(requestId);
        if (otp_request is null)
            return new()
            {
                Error = new() { Code = ErrorCodes.RegistrationRequestNotFound, Message = "Registration Request Not Found." }
            };
        if (otp_request.OTP != code || otp_request.User_Id.ToString() != userId)
            return new()
            {
                Error = new() { Code = ErrorCodes.InvalidOTP, Message = "Invalid OTP." }
            };
        var updateSuccess = await UpdateUserPropertyById(userId, "phone", otp_request.Phone);
        if (!updateSuccess)
            return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new() { Data = true, Message = "Phone updated successfully" };
    }
    public async Task<OperationResult<string>> UpdateUserEmail(string userId, string password, string newEmail)
    {
        if (await IsEmailUsed(newEmail))
            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.DbUniqueViolation,
                    Message = "Email is already used."
                }
            };

        var findUserRes = await FindUserById(userId);
        if (!findUserRes.IsSuccess) return new() { Error = findUserRes.Error };

        if (!BCrypt.Net.BCrypt.Verify(password, findUserRes.Data!.Password_Hash))
            return new() { Error = new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" } };

        // Compute OTP
        var otp = _otpManager.GenerateOTP();
        Guid requestId = Guid.NewGuid();
        var emailSubject = "تأكيد البريد الالكتروني لحساب تطبيق قيدها";
        var emailBody = await _mailingService.GenerateConfirmEmailBody(otp, requestId.ToString());
        var emailSendingRes = await _mailingService.SendEmailAsync(newEmail, emailSubject, emailBody);
        if (!emailSendingRes.IsSuccess)
            return emailSendingRes;

        // SAVE REQUEST DATA 
        await _updateEmailRequestRepo.AddAsync(new()
        {
            Id = requestId,
            Email = newEmail,
            OTP = otp,
            User_Id = Guid.Parse(userId),
            Created_On = DateTime.UtcNow
        });
        // RETURN SUCCESS
        return emailSendingRes;
    }

    public async Task<OperationResult<bool>> ConfirmEmailUpdate(string code, string requestId)
    {


        var otp_request = await _updateEmailRequestRepo.FindAsync(requestId);
        if (otp_request is null)
            return new()
            {
                Error = new() { Code = ErrorCodes.RegistrationRequestNotFound, Message = "Registration Request Not Found." }
            };
        if (otp_request.OTP != code)
            return new()
            {
                Error = new() { Code = ErrorCodes.InvalidOTP, Message = "Invalid OTP." }
            };
        var updateEmailSuccess = await UpdateUserPropertyById(otp_request.User_Id.ToString(), "email", otp_request.Email);
        var updateIsEmailConfirmedSuccess = await UpdateUserPropertyById(otp_request.User_Id.ToString(), "is_email_confirmed", true);

        if (!updateEmailSuccess || !updateIsEmailConfirmedSuccess)
            return new() { Error = new() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" } };
        return new() { Data = true, Message = "Email updated successfully" };
    }
}
