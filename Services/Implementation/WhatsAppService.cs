using Microsoft.Extensions.Options;
using Qydha.Helpers;
using Qydha.Models;

namespace Qydha.Services;

public class WhatsAppService : IMessageService
{
    private readonly WhatsAppSettings _whatsSettings;
    public WhatsAppService(IOptions<WhatsAppSettings> whatsSettings)
    {
        _whatsSettings = whatsSettings.Value;
    }
    public async Task<OperationResult<bool>> SendAsync(string phoneNum, string otp)
    {
        using HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _whatsSettings.Token);

        var body = new
        {
            messaging_product = "whatsapp",
            to = phoneNum,
            type = "template",
            template = new
            {
                name = "qydha_otp",
                language = new
                {
                    code = "ar"
                },
                components = new object[]
                {
                    new
                    {
                        type = "body",
                        parameters = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = otp
                            }
                        }
                    },
                    new
                    {
                        type = "button",
                        sub_type = "url",
                        index = "0",
                        parameters = new object[]
                        {
                            new
                            {
                                type = "payload",
                                payload = otp
                            }
                        }
                    }
                }
            }
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(new Uri(_whatsSettings.ApiUrl), body);
        if (!response.IsSuccessStatusCode)
            return new() { Error = new() { Code = ErrorCodes.OTPSendingError, Message = response.Content.ToString() ?? "unknown Error from whatsapp." } };

        return new OperationResult<bool>() { Data = true, Message = "Message Sent Successfully." };
    }
}
