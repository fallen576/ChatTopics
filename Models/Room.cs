namespace ChatTopics.Models
{
    public class Room
    {
        public string Name { get; set; }
        public List<string> Users { get; set; }
        public List<UserMessage> HistoricMessages { get; set; }
    }
}