using Microsoft.AspNetCore.Authentication.Cookies;

namespace ChatTopics
{
    public class CookieEvents : CookieAuthenticationEvents
    {
        private ChatDB _chatDB;

        public CookieEvents(ChatDB chatDB) => _chatDB = chatDB;

        public override Task SigningOut(CookieSigningOutContext context)
        {
            string username = context.HttpContext.User.Identity.Name;
            Console.WriteLine("Signing out " + username);
            _chatDB.LogoutUser(username);
            return base.SigningOut(context);
        }
    }
}
