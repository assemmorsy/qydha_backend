using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qydha.Controllers.Attributes;
using Qydha.Models;
using Qydha.Services;
using Qydha.Mappers;
namespace Qydha.Controllers;

[ApiController]
[Route("users/")]
[ValidateModel]
[ExceptionHandler]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    public UserController(IUserRepo userRepo)
    {
        _userRepo = userRepo;
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

        return Ok(new { updateUsernameRes.Message });
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
        return Ok(new { confirmPhoneUpdateRes.Message });
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

        string path = Path.Combine(Environment.CurrentDirectory, @"Templates\", fileName);
        var str = new StreamReader(path);
        var mailText = await str.ReadToEndAsync();
        str.Close();
        return Content(mailText, "text/html");
    }
}
