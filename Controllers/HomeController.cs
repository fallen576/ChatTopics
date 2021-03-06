using ChatTopics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ChatTopics.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatDB _chatDB;

        public HomeController(ILogger<HomeController> logger, ChatDB chatDB)
        {
            _logger = logger;
            _chatDB = chatDB;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_chatDB.GetRooms());
        }

        [HttpGet("/list")]
        public IActionResult GetRooms()
        {
            return Ok(_chatDB.GetRooms());
        }

        [HttpGet("/users")]
        public IActionResult GetUsers()
        {
            return Ok(_chatDB.GetUsers());
        }

        [HttpGet("/create")]
        public IActionResult CreateRoom(string roomName)
        {
            _chatDB.CreateRoom(roomName, User.Identity.Name);
            return Ok();
        }

        [HttpGet("/exists")]
        public IActionResult CheckRoomExists(string roomName)
        {
            if (_chatDB.RoomExists(roomName))
            {
                return Ok(new { Exists = true });
            }
            return Ok(new { Exists = false });
        }

        [HttpGet("/users/{roomName}")]
        public IActionResult GetUsersinRoom(string roomName)
        {
            List<User> users = _chatDB.GetUsersinRoom(roomName);
            if (users.Count > 0)
            {
                _logger.LogInformation("yes " + users.Count);
                return Ok(users);
            }
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}