namespace Qydha.Helpers;

public class JWTSettings
{
    public string SecretForKey { get; set; } = "string.Empty.secret";
    public string Issuer { get; set; } = "https://localhost:7002";
    public string Audience { get; set; } = "qydhaApi";

}
