using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SugarCenter.Controllers
{
    public class ShopController : Controller
    {
        private readonly ISugaringCentreAucklandElkRepository _elkRepository;

        public ShopController(ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
        }


        public async Task<IActionResult> Shop(int? categorySorting, int? sorting = 1)
        {
            var shopViewModel = new ShopViewModel();

            shopViewModel.ShopCategories = await _elkRepository.GetShopCategories();
            shopViewModel.ShopItems = await _elkRepository.GetShopItemsForCategory(categorySorting, sorting);

            return View(shopViewModel);
        }

        public async Task<IActionResult> SingleItem(int? productId)
        {
            if (productId == null)
            {
                return RedirectToAction("Shop");
            }

            var product = await _elkRepository.GetShopItem(productId);

            return View(product);
        }

        public IActionResult ShopCheckout()
        {
            return View();
        }
    }
}
