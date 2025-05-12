using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(object notification)
    {
        await Clients.All.SendAsync("ReceiveNotification", notification);
    }

    //public async Task SendNotificationToAdmin(object notification)
    //{
    //    await Clients.All.SendAsync("AdminReceiveNotification", notification);
    //}
}
