using ChatTopics.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatTopics.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private ChatDB _chatDB;

        public AccountController(ILogger<AccountController> logger, ChatDB chatDB)
        {
            _logger = logger;
            _chatDB = chatDB;
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [Route("auth")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("spalogin")]
        [HttpGet]
        public async Task<IActionResult> SpaLogin(string username)
        {
            if (_chatDB.UserExists(username))
            {
                return BadRequest(new { Message = "Username taken" });
            }
            _chatDB.CreateUser(username);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            return Ok();
        }

        [Route("login")]
        public async Task<IActionResult> LoginAsync(User user)
        {
            if (String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.UserName))
            {
                return Redirect("/");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation("signed in " + user.Email + " " + user.UserName);

            return Redirect("/");
        }
    }
}
