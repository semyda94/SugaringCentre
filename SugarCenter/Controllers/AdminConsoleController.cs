using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
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

        public AdminConsoleController (ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
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
            if (product != null)
            {
                await _elkRepository.CreateProduct(product);
            }
            else
            {
                
            }

            return RedirectToAction("Products");
        }

        #endregion


        
        public IActionResult Services()
        {
           return View();
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

        public IActionResult Staff()
        {
            return View();
        }
    }
}