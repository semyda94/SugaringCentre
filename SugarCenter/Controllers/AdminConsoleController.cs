using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;

namespace SugarCenter.Controllers
{
    public class AdminConsoleController : Controller
    {
        private readonly ISugaringCentreAucklandElkRepository _elkRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AdminConsoleController (ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository, IHostingEnvironment hostingEnvironment)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Home()
        {
            return View();
        }

        #region Category

        public IActionResult Category()
        {
            var categories = _elkRepository.GetShopCategories().GetAwaiter().GetResult();

            return View(categories);
        }

        public IActionResult DeleteCategory(int? id)
        {
            if (id.HasValue)
            {
                _elkRepository.DeleteCategory(id.Value);
            }
            return RedirectToAction("Category");
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult AddCategory(string newCategoryText)
        {
            _elkRepository.CreatCategory(newCategoryText).GetAwaiter().GetResult();
            return RedirectToAction("Category");
        }

        public async Task<JsonResult> GetCategory(string searchCategoryName)
        {
            var category = await _elkRepository.GetShopCategoriesForAc(searchCategoryName);
            var modifiedData = category.Select(x => new
            {
                id = x.CategoryId,
                text = x.Name
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }

        #endregion

        #region Products

        public async Task<IActionResult> Products()
        {
            var viewMovel = new ShopViewModel();

            viewMovel.Categories = await _elkRepository.GetShopCategories();
            viewMovel.Products = await _elkRepository.GetProducts();

            return View(viewMovel);
        }
        
        public IActionResult DeleteProduct(int? productId)
        {
            if (productId != null)
            {
                _elkRepository.DeleteProduct(productId).GetAwaiter().GetResult();
            }

            return RedirectToAction("Products");
        }

        public async Task<IActionResult> ProductConfiguration(int? productId)
        {
            return View(productId == null ? new Product{ProductId = -1} : await _elkRepository.GetShopItem(productId));
        }
        
        public JsonResult SaveCategorySelection(string categoryIds)
        {
            return Json(0, new JsonSerializerSettings());
        }
        
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> SaveProduct(Product product)
        {
            if (product.ProductId <= 0 )
            {
                var projectWebRootPath = _hostingEnvironment.WebRootPath;
                await _elkRepository.CreateProduct(product, projectWebRootPath);
            }
            else
            {
                await _elkRepository.UpdateProduct(product);
            }

            return RedirectToAction("Products");
        }

        #endregion

        #region Staff
        
        public async Task<IActionResult> Staff()
        {
            var staffList = await _elkRepository.GetStaffList();
            return View(staffList);
        }
        
        public async Task<IActionResult> DeleteStaff(int? staffId)
        {
            if (staffId != null)
            {
               await _elkRepository.DeleteStaff(staffId.Value);
            }

            return RedirectToAction("Staff");
        }
        
        public async Task<IActionResult> StaffConfiguration(int? staffId)
        {
            return View(staffId == null ? new Staff{StaffId = -1} : await _elkRepository.GetStaff(staffId.Value));
        }

        public async Task<IActionResult> SaveStaff(Staff staff)
        {
            if (staff.StaffId <= 0 )
            {
                var projectWebRootPath = _hostingEnvironment.WebRootPath;
                await _elkRepository.CreateStaff(staff);
            }
            else
            {
                await _elkRepository.UpdateStaff(staff);
            }
            
            return RedirectToAction("Staff");
        }

        #endregion
        
        public async Task<IActionResult> Services()
        {
            var services = await _elkRepository.GetServices();
           return View(services);
        }

        public async Task<IActionResult> DeleteService(int? serviceId)
        {
            if (serviceId != null)
                await _elkRepository.DeleteService(serviceId.Value);
                
            return RedirectToAction("Services");
        }
        

        //public IActionResult SaveProduct(string productName, string productDescription, decimal productPrice, int? productId)
        //{
        //    if (productId == null)
        //    {
        //        _elkRepository.CreateProduct(new Products {Name = productName, Desc = productDescription, Price = productPrice}).GetAwaiter().GetResult();
        //    }

        //    return RedirectToAction("Products");
        //}

        /*[Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> SaveProduct(Product item, List<IFormFile> ProductImg)
        {
            foreach (var i in ProductImg)
            {
                using (var stream = new MemoryStream())
                {
                    await i.CopyToAsync(stream);
                    //item.ProductImg = stream.ToArray();
                    await _elkRepository.CreateProduct(item);
                }
            }
            return RedirectToAction("Products");
        }*/
        
    }
}