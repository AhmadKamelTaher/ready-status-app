using Microsoft.AspNetCore.SignalR;

namespace ReadyStatusApp.Hubs
{
    public class StatusHub : Hub
    {
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        public async Task NotifyStatusChange(string userId, string userName, bool isReady)
        {
            await Clients.Group("Admins").SendAsync("ReceiveStatusUpdate", userId, userName, isReady);
        }
    }
}
