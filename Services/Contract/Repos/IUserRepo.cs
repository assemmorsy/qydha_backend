using Qydha.Entities;
using Qydha.Models;

namespace Qydha.Services;

public interface IUserRepo
{
    Task<OperationResult<User>> AddUser(User user);
    Task<OperationResult<User>> SaveUserFromRegistrationOTPRequest(RegistrationOTPRequest otpRequest);
    Task<OperationResult<User>> UpdateUser(User user);
    Task<bool> UpdateUserAvatar(string userId, string avatar_url, string avatar_path);
    Task<bool> IsUsernameUsed(string username);
    Task<bool> IsPhoneUsed(string phone);
    Task<OperationResult<User>> FindUserByUsername(string username);
    Task<OperationResult<User>> FindUserByEmail(string email);
    Task<OperationResult<User>> FindUserByPhone(string phone);
    Task<bool> UpdateUserLastLoginToNow(string userId);
    Task<OperationResult<User>> FindUserById(string userId);
    Task<bool> UpdateUserPropertyById<T>(string userId, string propName, T newValue);
    Task<OperationResult<bool>> UpdateUserPassword(string userId, string oldPassword, string newPassword);
    Task<OperationResult<bool>> UpdateUserUsername(string userId, string password, string newUsername);
    Task<OperationResult<string>> UpdateUserPhone(string userId, string password, string newPhone);
    Task<OperationResult<bool>> ConfirmPhoneUpdate(string userId, string code, string requestId);
    Task<OperationResult<string>> UpdateUserEmail(string userId, string password, string newEmail);
    Task<OperationResult<bool>> ConfirmEmailUpdate(string code, string requestId);


}
