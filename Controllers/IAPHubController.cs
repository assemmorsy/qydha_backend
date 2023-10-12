using Microsoft.AspNetCore.Mvc;
using Qydha.Controllers.Attributes;
using Qydha.Models;

namespace Qydha.Controllers;

[ApiController]
[Route("iaphub/")]
[ValidateModel]
[ExceptionHandler]
public class IAPHubController : ControllerBase
{
    [HttpPost]
    public IActionResult IApHubWebHook([FromBody] WebHookDto webHookDto)
    {
        if (!Request.Headers.TryGetValue("x-auth-token", out var authToken))
            return Unauthorized("x-auth-token header is missing.");

        string tokenValue = authToken.ToString();
        string webhookKey = "Nm8tAk4zxH1QsXZqXpwoHijwtjz1kEq";
        if (tokenValue != webhookKey) return Unauthorized("x-auth-token header is Wrong.");

        Console.WriteLine(webHookDto);

        return Ok("Token received and processed.");

    }

}
