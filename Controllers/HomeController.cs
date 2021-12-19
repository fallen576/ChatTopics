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

        [HttpGet("/create")]
        public IActionResult CreateRoom(string roomName)
        {
            _chatDB.CreateRoom(roomName);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}