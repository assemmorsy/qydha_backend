using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qydha.Controllers.Attributes;
using Qydha.Models;
using Qydha.Services;
using Qydha.Mappers;
using Microsoft.AspNetCore.JsonPatch;
namespace Qydha.Controllers;

[ApiController]
[Route("users/")]
[ValidateModel]
[ExceptionHandler]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    private readonly NotificationRepo _notificationRepo;

    public UserController(IUserRepo userRepo, NotificationRepo notificationRepo)
    {
        _userRepo = userRepo;
        _notificationRepo = notificationRepo;
    }
    [HttpGet("me/")]
    public async Task<IActionResult> GetUser()
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        var opRes = await _userRepo.FindUserById(userId);
        if (!opRes.IsSuccess)
            return NotFound(new Error() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" });

        var mapper = new UserMapper();
        return Ok(mapper.UserToUserDto(opRes.Data!));
    }

    [HttpPatch("me/update-password/")]
    public async Task<IActionResult> UpdateAuthorizedUserPassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        var updatePasswordRes = await _userRepo.UpdateUserPassword(userId, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!updatePasswordRes.IsSuccess)
            return BadRequest(updatePasswordRes.Error);

        return Ok(new { updatePasswordRes.Message });
    }


    [HttpPatch("me/update-username/")]
    public async Task<IActionResult> UpdateAuthorizedUsername([FromBody] ChangeUsernameDto changeUsernameDto)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        var updateUsernameRes = await _userRepo.UpdateUserUsername(userId, changeUsernameDto.Password, changeUsernameDto.NewUsername);
        if (!updateUsernameRes.IsSuccess)
            return BadRequest(updateUsernameRes.Error);
        var mapper = new UserMapper();
        return Ok(new { Data = mapper.UserToUserDto(updateUsernameRes.Data!), updateUsernameRes.Message });
    }

    [HttpPatch("me/update-phone/")]
    public async Task<IActionResult> UpdateAuthorizedPhone([FromBody] ChangePhoneDto changePhoneDto)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        var updatePhoneRes = await _userRepo.UpdateUserPhone(userId, changePhoneDto.Password, changePhoneDto.NewPhone);
        if (!updatePhoneRes.IsSuccess)
            return BadRequest(updatePhoneRes.Error);

        return Ok(new { Request_Id = updatePhoneRes.Data, updatePhoneRes.Message });
    }

    [HttpPost("me/confirm-phone-update/")]
    public async Task<IActionResult> ConfirmPhoneUpdate([FromBody] ConfirmPhoneDto confirmPhoneDto)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });

        var confirmPhoneUpdateRes = await _userRepo.ConfirmPhoneUpdate(userId, confirmPhoneDto.Code, confirmPhoneDto.RequestId);
        if (!confirmPhoneUpdateRes.IsSuccess)
            return BadRequest(confirmPhoneUpdateRes.Error);
        var mapper = new UserMapper();
        return Ok(new { Data = mapper.UserToUserDto(confirmPhoneUpdateRes.Data!), confirmPhoneUpdateRes.Message });
    }

    [HttpPatch("me/update-email")]
    public async Task<IActionResult> UpdateAuthorizedEmail([FromBody] ChangeEmailDto changeEmailDto)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        var updateEmailRes = await _userRepo.UpdateUserEmail(userId, changeEmailDto.Password, changeEmailDto.NewEmail);
        if (!updateEmailRes.IsSuccess)
            return BadRequest(updateEmailRes.Error);

        return Ok(new { updateEmailRes.Message });
    }

    [HttpGet("/confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailUpdate([FromQuery] ConfirmEmailDto confirmEmailDto)
    {
        var confirmEmailUpdateRes = await _userRepo.ConfirmEmailUpdate(confirmEmailDto.Code, confirmEmailDto.RequestId);
        string fileName;
        if (!confirmEmailUpdateRes.IsSuccess)
            fileName = "ConfirmFail.html";
        else
            fileName = "ConfirmSuccess.html";

        string path = Path.Combine(Environment.CurrentDirectory, @"Templates", fileName);
        var str = new StreamReader(path);
        var mailText = await str.ReadToEndAsync();
        str.Close();
        return Content(mailText, "text/html");
    }

    [HttpPatch("me/update-avatar")]
    public async Task<IActionResult> UpdateUserAvatar([FromForm] IFormFile file)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });

        var uploadUserPhotoRes = await _userRepo.UploadUserPhoto(userId, file);
        if (!uploadUserPhotoRes.IsSuccess)
            return BadRequest(uploadUserPhotoRes.Error);
        var mapper = new UserMapper();
        return Ok(new { Data = mapper.UserToUserDto(uploadUserPhotoRes.Data!), Message = uploadUserPhotoRes.Message });
    }

    [HttpPatch("me/")]
    public async Task<IActionResult> UpdateUserData([FromBody] JsonPatchDocument<UpdateUserDto> updateUserDtoPatch)
    {
        if (updateUserDtoPatch is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidInput, Message = "No Patch Data Found" });

        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        var getUserRes = await _userRepo.FindUserById(userId);
        if (!getUserRes.IsSuccess)
            return BadRequest(getUserRes.Error);

        var user = getUserRes.Data;

        var dto = new UpdateUserDto() { Name = user!.Name ?? "", BirthDate = user.Birth_Date };

        updateUserDtoPatch.ApplyTo(dto, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        user!.Name = dto.Name;
        user!.Birth_Date = dto.BirthDate;

        var updateUserRes = await _userRepo.UpdateUser(user);
        if (!updateUserRes.IsSuccess)
            return BadRequest(updateUserRes.Error);
        var mapper = new UserMapper();
        return Ok(new { Data = mapper.UserToUserDto(updateUserRes.Data!), Message = updateUserRes.Message });
    }

    [HttpDelete("me/")]
    public async Task<IActionResult> DeleteUser(DeleteUserDto deleteUserDto)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });

        var deleteUserRes = await _userRepo.DeleteUser(userId, deleteUserDto.Password);
        if (!deleteUserRes.IsSuccess)
            return BadRequest(deleteUserRes.Error);

        return Ok(new { deleteUserRes.Message });
    }

    [HttpGet("me/notifications")]
    public async Task<IActionResult> GetUserNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, [FromQuery] bool? isRead = null)
    {
        string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userIdStr is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        if (!Guid.TryParse(userIdStr, out Guid userId))
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id Incorrect" });
        var res = await _notificationRepo.GetAllNotificationsOfUserById(userId, pageSize, pageNumber, isRead);
        if (!res.IsSuccess) return BadRequest(res.Error);
        return Ok(new { res.Data, res.Message });
    }

    [HttpPatch("me/notifications/{notificationId}/mark-as-read/")]
    public async Task<IActionResult> MarkNotificationAsRead([FromRoute] int notificationId)
    {
        string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userIdStr is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        if (!Guid.TryParse(userIdStr, out Guid userId))
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id Incorrect" });
        var res = await _notificationRepo.MarkNotificationAsRead(userId, notificationId);
        if (!res.IsSuccess) return BadRequest(res.Error);
        return Ok(res.Message);
    }

    [HttpPatch("me/update-fcm-token")]
    public async Task<IActionResult> UpdateUsersFCMToken([FromBody] ChangeUserFCMTokenDto changeUserFCMTokenDto)
    {
        string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userIdStr is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        if (!Guid.TryParse(userIdStr, out Guid userId))
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id Incorrect" });
        if (!await _userRepo.UpdateUserPropertyById(userId.ToString(), "FCM_Token", changeUserFCMTokenDto.FCMToken))
            return BadRequest(new Error() { Code = ErrorCodes.UserNotFound, Message = "Failed To Update FCM token of the user" });
        return Ok(new { Message = "FCM Updated Successfully" });
    }
}
