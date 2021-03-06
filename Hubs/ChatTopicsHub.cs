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

        public async Task NotifyTopicCreate(string roomName)
        {
            _logger.LogInformation("topic has been created " + roomName);
            await Clients.All.SendAsync("TopicCreate", roomName);
        }

        public async Task<List<UserMessage>>? JoinRoom(string roomName)
        {
            _logger.LogInformation(Context.User.Identity.Name + " join room of " + roomName);
            _chatDB.AddUserToGroup(roomName, Context.User.Identity.Name);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.GroupExcept(roomName, new[] { Context.ConnectionId })
                .SendAsync("JoinTopic", Context.User.Identity.Name);
            return _chatDB.GetMessages(roomName);
        }

        public List<UserMessage>? GetHistoricMessages(string roomName)
        {
            return _chatDB.GetMessages(roomName);
        }

        public async Task<Task> LeaveRoom(string roomName)
        {
            _logger.LogInformation(Context.User.Identity.Name + " leave room of " + roomName);
            _chatDB.RemoveUserFromTopic(roomName, Context.User.Identity.Name);
            await Clients.GroupExcept(roomName, new[] { Context.ConnectionId })
                .SendAsync("LeaveTopic", Context.User.Identity.Name);
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public Task SendMessage(string message, string roomName)
        {
            _logger.LogInformation(Context.User.Identity.Name + " sending message to " + roomName + " with message of " + message);
            Room room = _chatDB.GetRooms().Find(r => r.Name == roomName) ?? new Room();
            UserMessage msg = new UserMessage
            {
                Message = message,
                UserName = Context.User.Identity.Name,
                TimeStamp = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")
            };
            _chatDB.UpdateUserLastActive(Context.User.Identity.Name);
            room.HistoricMessages.Add(msg);
            return Clients.Group(roomName).SendAsync("recieveMessage", msg);
            
            //return Clients.GroupExcept(roomName, new[] { Context.ConnectionId })
              //  .SendAsync("recieveMessage", message);
        }
    }
}
