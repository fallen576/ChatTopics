using ChatTopics.Models;

namespace ChatTopics
{
    public class ChatDB
    {
        private readonly ILogger<ChatDB> _logger;
        private static readonly List<Room> _rooms = new();

        public ChatDB(ILogger<ChatDB> logger) => _logger = logger;
        public List<String> GetRooms()
        {
            return _rooms.Select(room => room.Name).ToList();   
        }

        public Room? GetRoom(string room)
        {
            return _rooms.Find(r => r.Name == room);
        }

        public List<UserMessage>? CreateRoom(string roomName)
        {
            Room tmpRoom = new Room
            {
                Name = roomName,
                HistoricMessages = new List<UserMessage>()
            };
            _rooms.Add(tmpRoom);
            return _rooms?.Find(r => r.Name == roomName)?.HistoricMessages;
        }

        public List<UserMessage>? GetMessages(string room)
        {
            return _rooms?.Find(r => r.Name == room)?.HistoricMessages;
        }

        public void AddMessage(string message, string roomName, string userName)
        {
            _rooms?.Find(r => r.Name == roomName)?.HistoricMessages
                .Add(new UserMessage { Message = message, UserName = userName, TimeStamp = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")});
        }
    }
}
