namespace Qydha.Entities;

public class User
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
    public string? Password_Hash { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? Birth_Date { get; set; }
    public DateTime Created_On { get; set; }
    public DateTime? Last_Login { get; set; }
    public bool Is_Anonymous { get; set; }
    public bool Is_Phone_Confirmed { get; set; }
    public bool Is_Email_Confirmed { get; set; }
    public string? Avatar_Url { get; set; }
    public string? Avatar_Path { get; set; }
    public DateTime? Expire_Date { get; set; } = null;
    public int Free_Subscription_Used { get; set; } = 0;
    public string FCM_Token { get; set; } = string.Empty;
}
