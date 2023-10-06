using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Qydha.Helpers;

namespace Qydha.Services;

public class TokenManager
{
    private readonly JWTSettings _jwtSettings;
    public TokenManager(IOptions<JWTSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    public string Generate(List<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_jwtSettings.SecretForKey)
        );

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            DateTime.UtcNow,
            //TODO : GET THE NUMBER FROM CONFIG FILE
            DateTime.UtcNow.AddDays(10),
            signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

}
