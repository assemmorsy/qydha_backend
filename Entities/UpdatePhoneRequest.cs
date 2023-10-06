namespace Qydha.Entities;

public class UpdatePhoneRequest
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;
    public DateTime Created_On { get; set; } = DateTime.UtcNow;
    public Guid User_Id { get; set; }
}
