using Microsoft.AspNetCore.SignalR;

namespace labo.signalr.api.Hubs
{
    public class MyHub : Hub
    {
        public async Task UnMessageAuClientQuiFaitAppel()
        {
            await Clients.Caller.SendAsync("TaskList");


        }
    }
}
