using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SugarCenter.Controllers
{
    public class ShopController : Controller
    {
        private readonly ISugaringCentreAucklandElkRepository _elkRepository;
        private IMemoryCache _cache;

        public ShopController(ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository, IMemoryCache memoryCache)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
            _cache = memoryCache;
        }


        public async Task<IActionResult> Shop(int? categorySorting = -1, int? sorting = 1)
        {
            ShopViewModel shopViewModel = HttpContext.Session.Get<ShopViewModel>("ShopViewModel");

            if (shopViewModel != null && shopViewModel.CategorySorting == categorySorting &&
                shopViewModel.Sorting == sorting)
                return View(shopViewModel);

            if (shopViewModel == null)
                shopViewModel = new ShopViewModel();
            
            var shopCategoriesTask = _elkRepository.GetShopCategories();
            var shopItemsTask = _elkRepository.GetShopItemsForCategory(categorySorting, sorting);
            
            Task.WaitAll(shopCategoriesTask, shopItemsTask);

            shopViewModel.ShopCategories = shopCategoriesTask.Result;
            shopViewModel.ShopItems = shopItemsTask.Result;
            
            HttpContext.Session.Set<ShopViewModel>("ShopViewModel", shopViewModel);
            return View(shopViewModel);
        }

        public async Task<IActionResult> SingleItem(int? productId)
        {
            if (productId == null)
            {
                return RedirectToAction("Shop");
            }
            
            /*ShopViewModel shopViewModel = HttpContext.Session.Get<ShopViewModel>("ShopViewModel");
            
            if (shopViewModel == null || !shopViewModel.ShopItems.Exists(si => si.ShopItemId == productId))*/
                return View(await _elkRepository.GetShopItem(productId));
            
            /*return View(shopViewModel.ShopItems.Single(si => si.ShopItemId == productId).);*/
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
            
            return View(itemLists);
        }
    }
}
