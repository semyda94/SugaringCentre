using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SugarCenter.Classes;
using SugarCenter.Helpers;
using SugarCenter.Interfaces;
using SugarCenter.ViewModel;

namespace SugarCenter.Controllers
{
    public class BlogController : Controller
    {
        private readonly IConfiguration _configuration;
        public BlogController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var oAuthResponse = HttpContext.Session.Get<oAuthResponseWithToken>("AuthInfo");

            if (oAuthResponse == null)
            {
                return Redirect(InstagramBasicDisplayAPIHelper.GetAuthorisationUrl());
            }
            
            var blogViewModel = await GetDataForBlog(oAuthResponse);
            return View(blogViewModel);
        }
        
        public async Task<ActionResult> OAuth(string code)
        {
            var deserializedResponse = await InstagramBasicDisplayAPIHelper.GetToken(code);
            HttpContext.Session.Set<oAuthResponseWithToken>("AuthInfo", deserializedResponse);
            
            return RedirectToAction("Index");
        }

        private async Task<BlogViewModel> GetDataForBlog(oAuthResponseWithToken oAuthResponse)
        {
            var vm = new BlogViewModel();

            vm.UserData = await InstagramBasicDisplayAPIHelper.GetUserDate(oAuthResponse);
            vm.UserMediaResponse = await InstagramBasicDisplayAPIHelper.GetUserMedia(oAuthResponse);
            return vm;
        }
    }
}