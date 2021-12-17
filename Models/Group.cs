using System.ComponentModel.DataAnnotations;

namespace ChatTopics.Models
{
    public class Group
    {
        [Display(Name = "Group Id")]
        public int? Id { get; set; }
        [Display(Name = "Name")]
        public string? GroupName { get; set; }
    }
}