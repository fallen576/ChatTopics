using ChatTopics.Models;

namespace ChatTopics
{
    public class DeleteJob : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private ChatDB _chatDB;
        private Timer _timer = null;

        public DeleteJob(ILogger<DeleteJob> logger, ChatDB chatDB)
        {
            _logger = logger;
            _chatDB = chatDB;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting DeleteJob");
            _timer = new Timer(DeleteOldTopicsandUsers, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DeleteOldTopicsandUsers(object? state)
        {
            List<Room> rooms = _chatDB.GetRooms();
            foreach (Room room in rooms)
            {
                string name = room.Name;
                if (_chatDB.Delete(name))
                {
                    _logger.LogWarning($"The topic {name} has been inactive for 5 minutes and is being deleted.");
                    _chatDB.RemoveTopic(name);
                }
            }

            List<User> users = _chatDB.GetUsers();
            foreach (User user in users)
            {
                string name = user.UserName;
                if (_chatDB.DeleteUser(name))
                {
                    _logger.LogWarning($"The user {name} has been inactive for 5 minutes and is being deleted.");
                    _chatDB.LogoutUser(name);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stoping DeleteJob");
            return Task.CompletedTask;
        }
    }
}
