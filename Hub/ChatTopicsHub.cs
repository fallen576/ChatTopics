using ChatTopics.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace ChatTopics.Hub
{
    public interface IClientInteraction
    {
        Task ChatAction(Group chat);
    }
    public class ChatTopicsHub : Hub<IClientInteraction>
    {
        public void CreateGroup(string groupName)
        {
            this.CreateGroup(groupName);
        }

        public async void AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public void DeleteGroup(string groupName)
        {
            this.DeleteGroup(groupName);
        }
    }
}
