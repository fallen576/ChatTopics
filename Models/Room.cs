namespace ChatTopics.Models
{
    public class Room
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public DateTime Created { get; set; }
        public List<User> Users { get; set; }
        public List<UserMessage> HistoricMessages { get; set; }
    }
}