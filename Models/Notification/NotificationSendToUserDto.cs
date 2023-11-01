using System.ComponentModel.DataAnnotations;

namespace Qydha.Models;

public class NotificationSendToUserDto : NotificationSendDto
{
    [Required]
    public Guid UserId { get; set; }
}
