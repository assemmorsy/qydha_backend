namespace Qydha.Models;

public class GetUserDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsAnonymous { get; set; }
    public bool IsPhoneConfirmed { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? AvatarUrl { get; set; }
}

