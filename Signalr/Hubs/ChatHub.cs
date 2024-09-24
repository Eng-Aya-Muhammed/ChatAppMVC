using chatLab.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace SignlR.Hubs
{

    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, List<string>> UserConnections = new();
        private readonly ChatAppDbContext _db;

        public ChatHub(ChatAppDbContext db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            var email = Context.User.Identity.Name;  

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!UserConnections.ContainsKey(user.Id))
                {
                    UserConnections[user.Id] = new List<string>();
                }

                // Add the new connection ID to the list
                UserConnections[user.Id].Add(connectionId);
            }

            await base.OnConnectedAsync();
        }
        [Authorize]
        public async Task sendmessagetoreceiver(string sender, string receiverEmail, string message)
        {
            var userId = (await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == receiverEmail.ToLower()))?.Id;

            var loggedInUserEmail = Context.User?.Identity?.Name; 

            if (!string.IsNullOrEmpty(userId) && UserConnections.ContainsKey(userId))
            {
                foreach (var connectionId in UserConnections[userId])
                {
                    await Clients.Client(connectionId).SendAsync("MessageReceived", sender, message, loggedInUserEmail);
                }
            }
        }


        

        public async Task PersonIsTypingNotification(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                await Clients.Others.SendAsync("typingnotification", $"{name} is typing....");
            }
        }

        public async Task PersonStoppedTypingNotification()
        {
            await Clients.Others.SendAsync("typingnotification", "");
        }
        public async Task sendmessage(Message msg)
        {
            await Clients.All.SendAsync("newmessage", msg);
            _db.ChatMessages.Add(msg);
            await _db.SaveChangesAsync();
        }
        public async Task joingroup(string groupName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).SendAsync("newmember", userName, groupName);
        }

        public async Task sendtogroup(string userName, string message, string group)
        {
            var chatMessage = new Message
            {
                username = userName,
                messagetxt = message,
                groupname = group
            };
            await Clients.Group(group).SendAsync("groupmessage", userName, message, group);
            _db.ChatMessages.Add(chatMessage);
            await _db.SaveChangesAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;
            var email = Context.User.Identity.Name;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null && UserConnections.ContainsKey(user.Id))
            {
                UserConnections[user.Id].Remove(connectionId);

                if (UserConnections[user.Id].Count == 0)
                {
                    UserConnections.Remove(user.Id);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}