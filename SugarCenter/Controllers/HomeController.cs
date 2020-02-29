using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using SugarCenter.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
        
        public HomeController(ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
            
            
        }

        public IActionResult Index()
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
            return View("Index");
        
        }

        public JsonResult SubscribeWithPopUp(string email2)
        {
            _elkRepository.SubscribeForNews(email2).GetAwaiter().GetResult();
            return new JsonResult(new {Status = "Success", Result = "You email has been added to newslatter database"});
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
