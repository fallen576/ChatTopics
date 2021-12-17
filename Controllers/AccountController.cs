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

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
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

            return Redirect("/Group");
        }
    }
}
