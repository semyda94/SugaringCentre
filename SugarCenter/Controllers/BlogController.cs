using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SugarCenter.Classes;
using SugarCenter.Helpers;
using SugarCenter.ViewModel;

namespace SugarCenter.Controllers
{
    public class BlogController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var oAuthResponse = HttpContext.Session.Get<oAuthResponseWithToken>("AuthInfo");

            if (oAuthResponse == null)
            {
                return Redirect(InstagramBasicDisplayAPI.AutorisationUrl);
            }
            
            var blogViewModel = await GetDataForBlog(oAuthResponse);
            return View(blogViewModel);
        }
        
        public async Task<ActionResult> OAuth(string code)
        {
            var deserializedResponse = await InstagramBasicDisplayAPI.GetToken(code);
            HttpContext.Session.Set<oAuthResponseWithToken>("AuthInfo", deserializedResponse);
            
            return RedirectToAction("Index");
        }

        private async Task<BlogViewModel> GetDataForBlog(oAuthResponseWithToken oAuthResponse)
        {
            var vm = new BlogViewModel();

            vm.UserData = await InstagramBasicDisplayAPI.GetUserDate(oAuthResponse);
            vm.UserMediaResponse = await InstagramBasicDisplayAPI.GetUserMedia(oAuthResponse);
            return vm;
        }
    }
}