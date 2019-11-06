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


        public IActionResult Shop()
        {
            var shopViewModel = new ShopViewModel();

            shopViewModel.ShopCategories = _elkRepository.GetShopCategories().GetAwaiter().GetResult();
            shopViewModel.ShopItems = _elkRepository.GetShopItemsForCategory().GetAwaiter().GetResult();

            return View(shopViewModel);
        }

        public IActionResult SingleItem()
        {
            return View();
        }
    }
}
