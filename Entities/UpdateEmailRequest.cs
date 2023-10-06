namespace Qydha.Entities;

public class UpdateEmailRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;
    public DateTime Created_On { get; set; } = DateTime.UtcNow;
    public Guid User_Id { get; set; }
}
