using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.SignalR;

namespace labo.signalr.api.Hubs
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public class MyHub : Hub
    {
        ApplicationDbContext _context;

        public MyHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
            await Clients.Caller.SendAsync("TaskList", _context.UselessTasks.ToList());
        }

        public async Task AddTask(string taskText)
        {
            // Ajout de la task
            UselessTask uselessTask = new UselessTask()
            {
                Completed = false,
                Text = taskText
            };
            _context.UselessTasks.Add(uselessTask);
            await _context.SaveChangesAsync();

            // envois du message avec la liste (comprenant la nouvelle entrée)
            // "TaskList" doit être écrit pareil sur les newHubConnection.on("TaskList")
            // pcq c'est ce que le front-end écoute
            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToList());
        }

        public async Task CompletedTask(int id)
        {
            // completion of task
            UselessTask taskToModify = _context.UselessTasks.Single(t => t.Id == id);
            taskToModify.Completed = !taskToModify.Completed;
            await _context.SaveChangesAsync();

            // envois du message avec la liste (comprenant la nouvelle entrée)
            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToList());
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
