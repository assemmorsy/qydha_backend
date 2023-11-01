using System.Data;
using Dapper;
using Qydha.Entities;
using Qydha.Models;
namespace Qydha.Services;

public class NotificationRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly FCMService _fCMService;

    public NotificationRepo(IDbConnection dbConnection, FCMService fCMService)
    {
        _dbConnection = dbConnection;
        _fCMService = fCMService;
    }
    public async Task<OperationResult<Notification>> SendNotificationToUser(Notification notification, string userFCMToken)
    {

        if (!string.IsNullOrEmpty(userFCMToken))
        {
            await _fCMService.SendPushNotificationToUser(userFCMToken, notification.Title, notification.Description);
        }
        var sql = @"INSERT INTO 
                    Notification ( Title, Description,  Created_At, Action_Path, Action_Type, User_Id ) 
                    VALUES ( @Title, @Description,   NOW() , @Action_Path, @Action_Type, @User_Id  )
                    RETURNING Notification_Id;";
        var notificationId = await _dbConnection.QuerySingleAsync<int>(sql, notification);
        notification.Notification_Id = notificationId;
        return new() { Data = notification, Message = "Notification Saved Successfully" };
    }
    public async Task<OperationResult<bool>> SendNotificationToAllUsers(Notification notification)
    {
        await _fCMService.SendPushNotificationToAllUsers(notification.Title, notification.Description);
        var sql = @"
        INSERT INTO Notification (Title, Description, Created_At, Action_Path, Action_Type, User_Id)
            SELECT
            @Title,
            @Description,
            NOW(),
            @Action_Path,
            @Action_Type,
            Users.Id
            FROM Users;";
        await _dbConnection.ExecuteAsync(sql, notification);
        return new() { Data = true, Message = "Notification Saved to all users Successfully" };
    }
    public async Task<OperationResult<bool>> MarkNotificationAsRead(Guid userId, int notificationId)
    {
        var sql = @"update notification 
                        set read_at = now()
                        where notification_id = @notification_id and User_Id = @user_Id ;";
        await _dbConnection.ExecuteAsync(sql, new { notification_Id = notificationId, user_Id = userId });
        return new() { Data = true, Message = "Notification marked as read" };
    }
    public async Task<OperationResult<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null)
    {
        string cond = isRead is null ? "" : isRead.Value ? "and read_at is not null" : "and read_at is null";
        var sql = @$"
                    Select * from Notification 
                    where user_id = @userId {cond}
                    order by created_at desc 
                    LIMIT @limit OFFSET @offset ;
                    ";
        var notifications =
        await _dbConnection.QueryAsync<Notification>(sql,
            new { userId, limit = pageSize, offset = (pageNumber - 1) * pageSize });
        return new() { Data = notifications, Message = "users notification fetched." };
    }

}
