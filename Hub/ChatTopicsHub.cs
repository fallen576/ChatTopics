using ChatTopics.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatTopics.Hub
{
    public interface IClientInteraction
    {
        Task ChatAction(ChatTopic chat);
    }
    public class ChatTopicsHub : Hub<IClientInteraction>
    {

    }
}
