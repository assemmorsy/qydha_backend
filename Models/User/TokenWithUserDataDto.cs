
namespace Qydha.Models;

public class TokenWithUserDataDto
{
    public string Token { get; set; } = string.Empty;
    public GetUserDto UserData { get; set; } = new();
}
