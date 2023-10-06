using Twilio.Rest.Api.V2010.Account;

namespace Qydha.Services;

public interface ISMSService
{
    Task<MessageResource> SendAsync(string phoneNum, string body);
}
