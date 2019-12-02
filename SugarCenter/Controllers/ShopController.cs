using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

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

        public async Task<IActionResult> AddItemToCart(int productId)
        {
            var itemLists = HttpContext.Session.Get<List<ShopItem>>("CheckoutList");
            var item = await _elkRepository.GetShopItem(productId);

            if (itemLists == null)
                itemLists = new List<ShopItem>();

            itemLists.Add(item);

            HttpContext.Session.Set<List<ShopItem>>("CheckoutList", itemLists);

            return RedirectToAction("Shop");
        }

        public IActionResult ShopCheckout()
        {
            var itemLists = HttpContext.Session.Get<List<ShopItem>>("CheckoutList");
            
            return View();
        }
    }
}
