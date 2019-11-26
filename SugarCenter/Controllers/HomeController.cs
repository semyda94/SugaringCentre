using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using InstaSharp;
using Microsoft.AspNetCore.Mvc;
using SugarCenter.Models;
using InstaSharp.Models.Responses;
using Microsoft.IdentityModel.Protocols;
using System.Web;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;


namespace SugarCenter.Controllers
{

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class HomeController : Controller
    {
        private readonly ISugaringCentreAucklandElkRepository _elkRepository;

        private static string clientId = "535725053883034|Ho5VtL8KfcqEjb3V-8Bn5mLwZXU";
        private static string clietnSecret = "EAAHnPTWIspoBANmXxlxhwMLbXqteZBJZCMk3VkZBFRCSuHQdkatdLdTFSq3LYZC0CXwn7RgPoMH4ZCZBBC2Im2K5EL9nPZA80LLmvq6KeRwEuZB6sJfngwfjcOcmRYLoQjXOs3PZAb3ZArZC3TKg5w0ugiTaRXmZAoixaJ4rgU8Uo33bpAZDZD";
        private static string rederectUri = "https://localhost:44370/Home/OAuth";
        private static string realtimeUri = "";
        private InstagramConfig _config = new InstagramConfig(clientId, clietnSecret, rederectUri);

        public HomeController(ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Booking()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GetInTouchSend(string name, string email, string message)
        {
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com", // set your SMTP server name here
                Port = 587, // Port 
                EnableSsl = true,
                Credentials = new NetworkCredential("semykindmitrii94@gmail.com", "Nataliy1973")
            };


            using (var smtpMessage = new MailMessage("semykindmitrii94@gmail.com", "semykindmitrii94@gmail.com")
            {
                Subject = $"Support from '{email}'" ,
                Body = $"{message}"
            })
            {
                smtpClient.SendMailAsync(smtpMessage).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }

        public IActionResult SubscribeForNews(string email2)
        {
            _elkRepository.SubscribeForNews(email2).GetAwaiter().GetResult();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Blog()
        {
            var blogViewModel = new BlogViewModel();

            //var oAuthResponse = HttpContext.Session.Get<OAuthResponse>("InstaSharp.AuthInfo");

            //if (oAuthResponse == null)
            //{
            //    return RedirectToAction("Login");
            //}

            //var users = new InstaSharp.Endpoints.Users(_config, oAuthResponse);

            //blogViewModel.UserData = await users.GetSelf();
            //blogViewModel.MediaList = users.RecentSelf().GetAwaiter().GetResult().Data;
            var auth = new OAuth(_config);
            var oAuthResponse = await auth.RequestToken(clietnSecret);

            var users = new InstaSharp.Endpoints.Users(_config, oAuthResponse);

            blogViewModel.UserData = await users.GetSelf();
            blogViewModel.MediaList = users.RecentSelf().GetAwaiter().GetResult().Data;

            return View(blogViewModel);
        }

        public ActionResult Login()
        {
            var scopes = new List<OAuth.Scope>();
            scopes.Add(InstaSharp.OAuth.Scope.Basic);

            var link = InstaSharp.OAuth.AuthLink(_config.OAuthUri + "authorize", _config.ClientId, _config.RedirectUri, scopes, InstaSharp.OAuth.ResponseType.Code);

            return Redirect(link);
        }

        public async Task<ActionResult> OAuth(string code)
        {
            // add this code to the auth object
            var auth = new OAuth(_config);

            // now we have to call back to instagram and include the code they gave us
            // along with our client secret
            var oauthResponse = await auth.RequestToken(code);

            // both the client secret and the token are considered sensitive data, so we won't be
            // sending them back to the browser. we'll only store them temporarily.  If a user's session times
            // out, they will have to click on the authenticate button again - sorry bout yer luck.
            HttpContext.Session.Set<OAuthResponse>("InstaSharp.AuthInfo", oauthResponse);

            // all done, lets redirect to the home controller which will send some intial data to the app
            return RedirectToAction("Blog");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
