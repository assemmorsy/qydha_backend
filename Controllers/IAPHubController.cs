using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qydha.Controllers.Attributes;
using Qydha.Entities;
using Qydha.Helpers;
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

    public IAPHubController(IOptions<IAPHubSettings> iaphubSettings, SubscriptionRepo subscriptionRepo, IOptions<ProductsSettings> productSettings)
    {
        _iAPHubSettings = iaphubSettings.Value;
        _subscriptionRepo = subscriptionRepo;
        _productsSettings = productSettings.Value;
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
                return BadRequest(new Error() { Code = ErrorCodes.UnhandledIAPHubTransactionType, Message = "Unhandled IAPHub Transaction Type." });
        }


    }

}
