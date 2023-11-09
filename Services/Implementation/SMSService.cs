using Microsoft.Extensions.Options;
using Qydha.Helpers;
using Qydha.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Qydha.Services;

public class SMSService : IMessageService
{
    private readonly TwilioSettings _twilio;
    private readonly ILogger<SMSService> _logger;
    public SMSService(IOptions<TwilioSettings> twilio, ILogger<SMSService> logger)
    {
        _twilio = twilio.Value;
        _logger = logger;
    }
    public async Task<OperationResult<bool>> SendAsync(string phoneNum, string otp)
    {
        TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
        var result = await MessageResource.CreateAsync(
            body: $" رمز التحقق لتطبيق قيدها : {otp}",
            from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
            to: phoneNum
        );

        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            _logger.LogError("can't send the SMS", result);

            return new()
            {
                Error = new()
                {
                    Code = ErrorCodes.OTPSendingError,
                    Message = $"OTP Error Code :: {result.ErrorCode} => message :: {result.ErrorMessage}"
                }
            };
        }
        return new()
        {
            Data = true,
            Message = "Message sent successfully."
        };
    }
}
