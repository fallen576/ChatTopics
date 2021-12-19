using ChatTopics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatTopics.Hubs
{
    [Authorize]
    public class ChatTopicsHub : Hub
    {
        private readonly ILogger<ChatTopicsHub> _logger;
        private readonly ChatDB _chatDB;

        public ChatTopicsHub(ChatDB chatDB, ILogger<ChatTopicsHub> logger)
        {
            _chatDB = chatDB;
            _logger = logger;
        }
        public async Task<List<UserMessage>>? JoinRoom(string roomName)
        {
            _logger.LogInformation(Context.User.Identity.Name + " join room of " + roomName);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            return _chatDB.GetMessages(roomName);
        }

        public List<UserMessage>? GetHistoricMessages(string roomName)
        {
            return _chatDB.GetMessages(roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            _logger.LogInformation(Context.User.Identity.Name + " leave room of " + roomName);
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public Task SendMessage(string message, string roomName)
        {
            _logger.LogInformation(Context.User.Identity.Name + " sending message to " + roomName + " with message of " + message);
            _chatDB.AddMessage(message, roomName, Context.User.Identity.Name);
            return Clients.GroupExcept(roomName, new[] { Context.ConnectionId })
                .SendAsync("recieveMessage", message);
        }
    }
}
