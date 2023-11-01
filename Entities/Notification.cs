namespace Qydha.Entities;

public class Notification
{
    public int Notification_Id { get; set; }
    public Guid User_Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? Read_At { get; set; }
    public DateTime Created_At { get; set; }
    public string Action_Path { get; set; } = string.Empty;
    public NotificationActionType Action_Type { get; set; }
}

public enum NotificationActionType
{
    NoAction = 1,
    GoToURL = 2,
    GoToScreen = 3
}
