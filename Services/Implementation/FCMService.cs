using FirebaseAdmin.Messaging;

namespace Qydha.Services;

public class FCMService
{
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
        var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
        // TODO log errors
        Console.WriteLine(res);
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
        var res = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
        // TODO log errors
        // Console.WriteLine(res);
    }
    public async Task SendPushNotificationToGroupOfUsers(string topicName, string title, string body)
    {
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
        // TODO log errors
        // Console.WriteLine(res);
    }
}
