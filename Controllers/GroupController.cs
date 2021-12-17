using ChatTopics.Hub;
using ChatTopics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatTopics.Controllers
{
    public class GroupController : Controller
    {
        public static List<Group> GroupNames = new List<Group>();
        private readonly IHubContext<ChatTopicsHub> _hubContext;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IHubContext<ChatTopicsHub> hubContext, ILogger<GroupController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("Index", GroupNames);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("GroupName")]Group group)
        {
            _logger.LogInformation(group.GroupName);
            if (!GroupNames.Contains(group))
            {
                GroupNames.Add(group);
            }
            
            return Redirect("/Group");
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            Group group = GroupNames.FirstOrDefault(group => group.Id.Equals(id));
            if (group != null)
            {
                GroupNames.Remove(group);
            }
            return View();
        }
    }
}
