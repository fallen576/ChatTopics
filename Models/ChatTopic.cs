using System.ComponentModel.DataAnnotations;

namespace ChatTopics.Models
{
    public class ChatTopic
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? GroupName { get; set; }
        public string?  Message { get; set; }
    }
}