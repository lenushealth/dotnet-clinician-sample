namespace Clinician.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Models;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;

    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("login")]
        public async Task Login()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                await this.HttpContext.ChallengeAsync(new AuthenticationProperties()
                {
                    RedirectUri = this.Url.Action("Index")
                });
            }
        }

        [Route("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme );
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Route("error")]
        public IActionResult Error(string msg)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = msg});
        }
    }
}
