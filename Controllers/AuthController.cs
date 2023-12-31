using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qydha.Controllers.Attributes;
using Qydha.Models;
using Qydha.Services;

namespace Qydha.Controllers;

[ApiController]
[Route("/auth")]
[ValidateModel]
public class AuthController : ControllerBase
{
    private readonly IAuthRepo _authRepo;

    public AuthController(IAuthRepo authRepo)
    {
        _authRepo = authRepo;
    }

    [HttpPost("login-anonymous/")]
    public async Task<IActionResult> LoginAsAnonymous([FromBody] UserLoginAnonymousDto userLoginAnonymousDto)
    {
        var opRes = await _authRepo.LoginAsAnonymousAsync(userLoginAnonymousDto);
        if (!opRes.IsSuccess)
            return BadRequest(opRes.Error);
        return Ok(new { token = opRes.Data, opRes.Message });
    }

    [HttpPost("register/")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegisterDTO)
    {
        var opRes = await _authRepo.RegisterAsync(userRegisterDTO, null);
        if (!opRes.IsSuccess)
            return BadRequest(opRes.Error);
        return Ok(new { RequestId = opRes.Data, opRes.Message });
    }


    [HttpPost("register-anonymous/")]
    [Authorize]
    public async Task<IActionResult> RegisterAnonymous([FromBody] UserRegisterDTO userRegisterDTO)
    {
        string? userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        string? isAnonymousStr = User.Claims.FirstOrDefault(c => c.Type == "isAnonymous")?.Value;

        if (isAnonymousStr is null || isAnonymousStr != "True" || userId is null)
            return BadRequest(new Error() { Code = ErrorCodes.AnonymousUserTokenNotProvided, Message = "Anonymous User Token Not Provided Correctly." });

        var opRes = await _authRepo.RegisterAsync(userRegisterDTO, userId);
        if (!opRes.IsSuccess)
            return BadRequest(opRes.Error);
        return Ok(new { RequestId = opRes.Data, opRes.Message });
    }

    [HttpPost("login/")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var opRes = await _authRepo.Login(userLoginDto);
        if (!opRes.IsSuccess)
            return BadRequest(opRes.Error);
        return Ok(new { opRes.Data, opRes.Message });
    }

    [HttpPost("confirm-registration-with-phone/")]
    public async Task<IActionResult> ConfirmRegistrationWithPhone([FromBody] ConfirmPhoneDto confirmPhoneDto)
    {
        var opRes = await _authRepo.ConfirmRegistrationWithPhone(confirmPhoneDto.Code, confirmPhoneDto.RequestId);
        if (!opRes.IsSuccess)
            return BadRequest(opRes.Error);
        return Ok(new { opRes.Data, opRes.Message });
    }

    [Authorize]
    [HttpPost("logout/")]
    public async Task<IActionResult> Logout()
    {
        string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userIdStr is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id not provided" });
        if (!Guid.TryParse(userIdStr, out Guid userId))
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token user id Incorrect" });
        var logoutRes = await _authRepo.Logout(userId);
        if (!logoutRes.IsSuccess)
            return BadRequest(logoutRes.Error);
        return Ok(new { logoutRes.Message });
    }

    [HttpGet()]
    public IActionResult TestDeploy()
    {
        return Ok(new { Message = "Deployed. ✔️✔️" });
    }


}

