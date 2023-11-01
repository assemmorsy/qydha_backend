using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qydha.Controllers.Attributes;
using Qydha.Models;
using Qydha.Services;

namespace Qydha.Controllers;

[ApiController]
[Route("notifications/")]
[ValidateModel]
[ExceptionHandler]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly NotificationRepo _notificationRepo;
    private readonly IUserRepo _userRepo;

    public NotificationController(IUserRepo userRepo, NotificationRepo notificationRepo)
    {
        _notificationRepo = notificationRepo;
        _userRepo = userRepo;
    }

    [HttpPost("send-to-user/")]
    public async Task<IActionResult> SendNotificationToUser([FromBody] NotificationSendToUserDto dto)
    {
        var getUserRes = await _userRepo.FindUserById(dto.UserId.ToString());
        if (!getUserRes.IsSuccess)
            return BadRequest(new { getUserRes.Error });
        var res = await _notificationRepo.SendNotificationToUser(new Entities.Notification()
        {
            Title = dto.Title,
            Description = dto.Description,
            Action_Path = dto.Action_Path,
            Action_Type = dto.Action_Type,
            Created_At = DateTime.Now,
            User_Id = dto.UserId
        }, getUserRes.Data!.FCM_Token ?? "");
        if (!res.IsSuccess)
            return BadRequest(res.Error);
        return Ok(res.Message);
    }

    [HttpPost("send-to-all-users/")]
    public async Task<IActionResult> SendNotificationToAllUsers([FromBody] NotificationSendDto dto)
    {
        var res = await _notificationRepo.SendNotificationToAllUsers(new Entities.Notification()
        {
            Title = dto.Title,
            Description = dto.Description,
            Action_Path = dto.Action_Path,
            Action_Type = dto.Action_Type,
            Created_At = DateTime.Now,
        });
        if (!res.IsSuccess)
            return BadRequest(res.Error);
        return Ok(res.Message);
    }

}
