using Microsoft.Extensions.Options;
using OtpNet;
using Qydha.Helpers;

namespace Qydha.Services;

public class OtpManager
{
    private readonly Totp _totp;

    public OtpManager(IOptions<OTPSettings> OTPSettings)
    {
        _totp = new Totp(Base32Encoding.ToBytes(OTPSettings.Value.Secret)); ;
    }
    public string GenerateOTP()
    {
        return _totp.ComputeTotp();
    }


}
