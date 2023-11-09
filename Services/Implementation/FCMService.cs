using FirebaseAdmin.Messaging;

namespace Qydha.Services;

public class FCMService
{
    private readonly ILogger<FCMService> _logger;
    public FCMService(ILogger<FCMService> logger)
    {
        _logger = logger;
    }
    public async Task SendPushNotificationToUser(string userFcmToken, string title, string body)
    {
        var msg = new Message()
        {
            Token = userFcmToken,
            Notification = new Notification()
            {
                Title = title,
                Body = body
            }
        };
        try
        {
            var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
        }
        catch (FirebaseMessagingException exp)
        {
            switch (exp.MessagingErrorCode)
            {
                case MessagingErrorCode.Unregistered:
                    // TODO :: DELETE THIS FCM TOKEN
                    break;
                default:
                    _logger.LogError("unhandled error in FCM sendToUser", exp);
                    break;
            }
        }
    }
    public async Task SendPushNotificationToAllUsers(string title, string body)
    {
        var msg = new Message()
        {
            Topic = "all",
            Notification = new Notification()
            {
                Title = title,
                Body = body
            }
        };
        try
        {
            var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
        }
        catch (Exception exp)
        {
            _logger.LogError("unhandled error in FCM sendToAllUser", exp);
        }
    }
    public async Task SendPushNotificationToGroupOfUsers(string topicName, string title, string body)
    {
        //TODO Handle send to group of users 
        var msg = new Message()
        {
            Topic = topicName,
            Notification = new Notification()
            {
                Title = title,
                Body = body
            }
        };
        var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);

    }
}

