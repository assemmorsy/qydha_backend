using Microsoft.Extensions.Options;
using Qydha.Helpers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Qydha.Services;

public class SMSService : ISMSService
{
    private readonly TwilioSettings _twilio;
    public SMSService(IOptions<TwilioSettings> twilio)
    {
        _twilio = twilio.Value;
    }
    public async Task<MessageResource> SendAsync(string phoneNum, string body)
    {
        TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
        var result = await MessageResource.CreateAsync(
            body: body,
            from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
            to: phoneNum
        );
        return result;
    }
}
