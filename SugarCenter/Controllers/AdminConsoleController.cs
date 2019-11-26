using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

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

        public IActionResult Categories()
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
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public IActionResult AddCategory(string newCategoryText)
        {
            _elkRepository.CreatCategory(newCategoryText).GetAwaiter().GetResult();
            return RedirectToAction("Categories");
        }

        public IActionResult Products()
        {
            var viewMovel = new ShopViewModel();

            viewMovel.ShopCategories = _elkRepository.GetShopCategories().GetAwaiter().GetResult();
            viewMovel.ShopItems = _elkRepository.GetShoItems().GetAwaiter().GetResult();

            return View(viewMovel);
        }

        public IActionResult AddEditProduct(int? productId)
        {
             if (productId == null)
            {
                return View(new ShopItem{ShopItemId = -1});
            }
            else
            {
                return View(_elkRepository.GetShopItem(productId).GetAwaiter().GetResult());
            }
        }

        public IActionResult SaveProduct(string productName, string productDescription, decimal productPrice, int? productId)
        {
            if (productId == null)
            {
                _elkRepository.CreateProduct(new ShopItem {Name = productName, Desc = productDescription, Price = productPrice}).GetAwaiter().GetResult();
            }

            return RedirectToAction("Products");
        }

        public IActionResult DeleteProduct(int? productId)
        {
            if (productId != null)
            {
                _elkRepository.DeleteProduct(productId).GetAwaiter().GetResult();
            }

            return RedirectToAction("Products");
        }

        public IActionResult Staff()
        {
            return View();
        }
    }
}