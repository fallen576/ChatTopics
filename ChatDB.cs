using ChatTopics.Models;
using Microsoft.AspNetCore.Authentication;

namespace ChatTopics
{
    public class ChatDB
    {
        private readonly ILogger<ChatDB> _logger;
        private static readonly List<Room> _rooms = new();
        //private static readonly List<String> _users = new();
        private static readonly List<ChatTopics.Models.User> _currentUsers = new();
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;

        public ChatDB(ILogger<ChatDB> logger) => _logger = logger;
        public List<Room> GetRooms()
        {
            return _rooms.ToList();   
        }

        public Room? GetRoom(string room)
        {
            return _rooms.Find(r => r.Name == room);
        }

        public bool RoomExists(string roomName)
        {
            return _rooms.Exists(r => r.Name == roomName);
        }

        public List<UserMessage>? CreateRoom(string roomName, string username)
        {
            Room tmpRoom = new Room
            {
                Name = roomName,
                HistoricMessages = new List<UserMessage>(),
                Owner = username,
                Created = DateTime.Now,
                Users = new List<User>
                {
                    new User{ 
                        UserName = username,
                        LastActive = DateTime.Now
                    }
                }
            };
            if (!_rooms.Exists(r => r.Name == roomName))
            {
                _rooms.Add(tmpRoom);
            }

            UpdateUserLastActive(username);

            return _rooms?.Find(r => r.Name == roomName)?.HistoricMessages;
        }

        public void RemoveUserFromTopic(string roomName, string? name)
        {
            _rooms.Find(r => r.Name == roomName)?.Users.RemoveAll(u => u.UserName == name);
        }

        public void AddUserToGroup(string roomName, string username)
        {
            if (RoomExists(roomName) && !GetUsersinRoom(roomName).Exists(u => u.UserName == username))
            {
                _rooms.Find(r => r.Name == roomName)?.Users
                    .Add( new User 
                    { 
                        UserName = username,
                        LastActive = DateTime.Now 
                    });
                UpdateUserLastActive(username);
            }
        }

        public List<UserMessage>? GetMessages(string room)
        {
            return _rooms?.Find(r => r.Name == room)?.HistoricMessages;
        }

        public void AddMessage(string message, string roomName, string userName)
        {
            //update timer on room so it does not get deleted
            Room room = _rooms.Find(r => r.Name == roomName) ?? new Room();
            room.HistoricMessages
                .Add(new UserMessage { Message = message, UserName = userName, TimeStamp = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") });

            UpdateUserLastActive(userName);
        }

        public void UpdateUserLastActive(string userName)
        {
            var obj = _currentUsers.FirstOrDefault(u => u.UserName == userName);
            if (obj != null)
            {
                _logger.LogInformation("updating last active for " + userName);
                obj.LastActive = DateTime.Now;
            }
        }

        public bool Delete(string roomName)
        {

            if (!RoomExists(roomName))
                return false;

            Room room = _rooms.Find(x => x.Name == roomName);

            //if the room was created more than 5 mins ago and nobody has used it, delete it
            if (room.HistoricMessages.Count == 0)
            {
                if (room.Created < DateTime.Now.AddMinutes(-5))
                {
                    return true;
                }
                return false;
            }

            //if nobody has used it in the last 5 mins, delete it
            UserMessage lastMessage = room.HistoricMessages.OrderByDescending(x => x.TimeStamp).First();
            DateTime lastPost = DateTime.Parse(lastMessage.TimeStamp);//parse timestamp, why did i do this to myself

            //_logger.LogInformation($"lastPost {lastPost} > {DateTime.Now.AddMinutes(-5)}");

            if (lastPost < DateTime.Now.AddMinutes(-5))
            {
                return true;
            }
            return false;
        }

        public bool DeleteUser(string userName)
        {
            if (UserExists(userName))
            {
                var user = _currentUsers.FirstOrDefault(u =>u.UserName == userName);
                if (user != null && user.LastActive < DateTime.Now.AddMinutes(-1))
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveTopic(string roomName)
        {
            _rooms.RemoveAll(x => x.Name == roomName);
        }

        public void RemoveUser(string userName)
        {
            _currentUsers.RemoveAll(u => u.UserName == userName);
        }

        public bool UserExists(string username)
        {
            //return _users.Contains(username);
            return _currentUsers.Exists(x => x.UserName == username);
        }

        public void CreateUser(string username)
        {
            //_users.Add(username);
            _currentUsers.Add(
                new User
                {
                    UserName = username,
                    LastActive = DateTime.Now
                });
        }

        public List<User> GetUsers()
        {
            return _currentUsers.ToList();
        }

        public List<User> GetUsersinRoom(string roomName)
        {
            List<User> users = new();
            if (RoomExists(roomName))
            {
                users = _rooms.Where(r => r.Name == roomName)
                    .Select(u => u.Users)
                    .First()
                    .ToList();
            }
            return users;
        }

        public async Task LogoutUser(string username)
        {
            _logger.LogInformation($"logging out {username}. total users are {_currentUsers.Count}");
            RemoveUser(username);
            await _httpContext.SignOutAsync();
            _rooms.RemoveAll(room => room.Owner == username);
        }
    }
}
