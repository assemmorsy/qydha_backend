using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qydha.Controllers.Attributes;
using Qydha.Entities;
using Qydha.Helpers;
using Qydha.Mappers;
using Qydha.Models;
using Qydha.Services;

namespace Qydha.Controllers;

[ApiController]
[Route("iaphub/")]
[ValidateModel]
[ExceptionHandler]
public class IAPHubController : ControllerBase
{
    private readonly IAPHubSettings _iAPHubSettings;
    private readonly SubscriptionRepo _subscriptionRepo;
    private readonly ProductsSettings _productsSettings;
    private readonly IUserRepo _userRepo;

    public IAPHubController(IUserRepo userRepo, IOptions<IAPHubSettings> iaphubSettings, SubscriptionRepo subscriptionRepo, IOptions<ProductsSettings> productSettings)
    {
        _iAPHubSettings = iaphubSettings.Value;
        _subscriptionRepo = subscriptionRepo;
        _productsSettings = productSettings.Value;
        _userRepo = userRepo;
    }
    [HttpPost]
    public async Task<IActionResult> IApHubWebHook([FromBody] WebHookDto webHookDto)
    {
        if (!Request.Headers.TryGetValue("x-auth-token", out var authToken))
            return Unauthorized(new { Error = new Error() { Code = ErrorCodes.InvalidIAPHupToken, Message = "x-auth-token header is Missing" } });
        string tokenValue = authToken.ToString();
        if (tokenValue != _iAPHubSettings.XAuthToken) return Unauthorized(new { Error = new Error() { Code = ErrorCodes.InvalidIAPHupToken, Message = "x-auth-token header is wrong." } });

        switch (webHookDto.Type)
        {
            case "purchase":
                // TODO :: log  that ISSue
                if (!_productsSettings.ProductsSku.TryGetValue(webHookDto.Data!.ProductSku, out int numberOfDays))
                    return BadRequest(new Error() { Code = ErrorCodes.InvalidInput, Message = "Invalid Product sku" });

                var purchase = new Purchase()
                {
                    IAPHub_Purchase_Id = webHookDto.Data!.Id,
                    User_Id = webHookDto.Data!.UserId,
                    Type = webHookDto.Type,
                    Purchase_Date = webHookDto.Data!.PurchaseDate,
                    ProductSku = webHookDto.Data!.ProductSku,
                    Number_Of_Days = numberOfDays
                };
                var saveRes = await _subscriptionRepo.AddPurchase(purchase);
                if (!saveRes.IsSuccess) return BadRequest(saveRes.Error);
                return Ok(saveRes.Message);
            default:
                //TODO log any type that should be handled
                return Ok();
                // return BadRequest(new Error() { Code = ErrorCodes.UnhandledIAPHubTransactionType, Message = "Unhandled IAPHub Transaction Type." });
        }
    }

    [HttpPost("free_30/")]
    [Authorize]
    public async Task<IActionResult> SubscribeInFree()
    {
        string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (userIdStr is null)
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token : user id not provided" });

        if (!Guid.TryParse(userIdStr, out Guid userId))
            return BadRequest(new Error() { Code = ErrorCodes.InvalidToken, Message = "Invalid token : invalid user id " });

        var purchase = new Purchase()
        {
            IAPHub_Purchase_Id = Guid.NewGuid().ToString(),
            User_Id = userId,
            Type = "purchase",
            Purchase_Date = DateTime.Now,
            ProductSku = "free_30",
            Number_Of_Days = 30
        };
        var saveRes = await _subscriptionRepo.AddPurchase(purchase);
        if (!saveRes.IsSuccess) return BadRequest(saveRes.Error);
        var getUserRes = await _userRepo.FindUserById(userIdStr);
        if (!getUserRes.IsSuccess) return BadRequest(getUserRes.Error);
        var mapper = new UserMapper();
        return Ok(new { Data = mapper.UserToUserDto(getUserRes.Data!) });

    }

}
