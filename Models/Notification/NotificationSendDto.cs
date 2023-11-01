using System.ComponentModel.DataAnnotations;
using Qydha.Entities;

namespace Qydha.Models;

public class NotificationSendDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;


    [Required]
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(350)]
    public string Action_Path { get; set; } = string.Empty;
   
    [Required]
    public NotificationActionType Action_Type { get; set; }
}
