using Microsoft.AspNetCore.Mvc;

namespace ChatTopics.Controllers
{
    public class ChatTopicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
