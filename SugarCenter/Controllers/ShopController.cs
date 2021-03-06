﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SugarCenter.Classes;
using SugarCenter.Models;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SugarCenter.Controllers
{
    [Produces("application/json")]
    public class ShopController : Controller
    {
        private readonly ISugaringCentreAucklandElkRepository _elkRepository;
        private IMemoryCache _cache;
        
        private const int PageSize = 12;

        public ShopController(ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository, IMemoryCache memoryCache)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
            _cache = memoryCache;
        }


        public async Task<IActionResult> Index(int? categorySorting = -1, int? sorting = 1, int? pageNumber = 1)
        {
            ShopViewModel shopViewModel = HttpContext.Session.Get<ShopViewModel>("ShopViewModel");

            // if (shopViewModel != null && shopViewModel.CategorySorting == categorySorting &&
            //     shopViewModel.Sorting == sorting && shopViewModel.PageIndex == pageNumber)
            //     return View(shopViewModel);

            if (shopViewModel == null)
            {
                shopViewModel = new ShopViewModel();

                shopViewModel.Sorting = sorting ?? -1;
                shopViewModel.CategorySorting = categorySorting ?? 1;

                var shopCategoriesTask = _elkRepository.GetListOfCategories();
                var shopItemsTask = _elkRepository.GetProductsForCategoryAndSort(categorySorting, sorting);

                Task.WaitAll(shopCategoriesTask, shopItemsTask);

                shopViewModel.Categories = shopCategoriesTask.Result;
                shopViewModel.Products = shopItemsTask.Result;

                shopViewModel.PageIndex = 1;
                shopViewModel.PageSize = PageSize;
                shopViewModel.TotalPages = (int) Math.Ceiling(shopItemsTask.Result.Count / (double) PageSize);
            }

            if (shopViewModel.CategorySorting != categorySorting || shopViewModel.Sorting != sorting)
            {
                var shopCategoriesTask = _elkRepository.GetListOfCategories();
                var shopItemsTask = _elkRepository.GetProductsForCategoryAndSort(categorySorting, sorting);

                Task.WaitAll(shopCategoriesTask, shopItemsTask);

                shopViewModel.Sorting = sorting ?? -1;
                shopViewModel.CategorySorting = categorySorting ?? 1;

                shopViewModel.Categories = shopCategoriesTask.Result;
                shopViewModel.Products = shopItemsTask.Result;

                shopViewModel.PageIndex = 1;
                shopViewModel.PageSize = PageSize;
                shopViewModel.TotalPages = (int) Math.Ceiling(shopItemsTask.Result.Count / (double) PageSize);
            }

            if (shopViewModel.PageIndex != pageNumber)
            {
                shopViewModel.PageIndex = (pageNumber ?? 1) >= 1 && (pageNumber ?? 1) <= shopViewModel.TotalPages
                    ? (pageNumber ?? 1)
                    : shopViewModel.PageIndex;
            }

            HttpContext.Session.Set("ShopViewModel", shopViewModel);
            return View(shopViewModel);
        }

        public async Task<IActionResult> SingleItem(int? productId)
        {
            if (productId == null)
            {
                return RedirectToAction("Index");
            }
            
            // ShopViewModel shopViewModel = HttpContext.Session.Get<ShopViewModel>("ShopViewModel");
            SingleItemViewModel singleItemViewModel = new SingleItemViewModel();

            // if (shopViewModel == null || !shopViewModel.Products.Exists(x => x.ProductId == productId))
            // {
                singleItemViewModel.Product = await _elkRepository.GetProductById(productId);   
            // }
            // else
            // {
            //     singleItemViewModel.Product = shopViewModel.Products.Single(p => p.ProductId == productId);
            // }

            var relatedCategoriesIds = singleItemViewModel.Product.ProductCategory.Select(x => x.CategoryId);
            
            singleItemViewModel.RelatiedProducts = _elkRepository.GetRelatedProducts(productId.Value, relatedCategoriesIds);

            return View(singleItemViewModel);
        }

        public async Task<JsonResult> AddItemToCartJson(int productId)
        {
            Product productToAdd; 
        
            var products = HttpContext.Session.Get<List<Product>>("CheckoutList");
        
            if (products == null)
                products = new List<Product>();
            
            productToAdd = await _elkRepository.GetProductById(productId);
            
            products.Add(productToAdd);
            
            HttpContext.Session.Set("CheckoutList", products);
            
            return Json(true, new JsonSerializerSettings());
        }
        
        public async Task<IActionResult> AddItemToCart(Product product)
        {
            Product productToAdd = null;

            var products = HttpContext.Session.Get<List<Product>>("CheckoutList");

            if (products == null)
                products = new List<Product>();

            ShopViewModel shopViewModel = HttpContext.Session.Get<ShopViewModel>("ShopViewModel");

            if (shopViewModel != null && shopViewModel.Products.Any())
            {
                productToAdd = shopViewModel.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            }

            if (product == null || shopViewModel == null)
            {
                productToAdd = await _elkRepository.GetProductById(product.ProductId);
            }

            for (var i = 0; i < product.Qty; ++i)
            {
                products.Add(productToAdd);
            }

            HttpContext.Session.Set("CheckoutList", products);

            return RedirectToAction("Index");
        }

        public IActionResult ShopCheckout()
        {
            var itemLists = HttpContext.Session.Get<List<Product>>("CheckoutList");

            if (itemLists == null || itemLists.Count <= 0)
                return RedirectToAction("Index");

            return View(itemLists);
        }

        public IActionResult RemoveItemFromCart(int? productId)
        {
            var itemLists = HttpContext.Session.Get<List<Product>>("CheckoutList");

            if (itemLists == null || productId == null)
            {
                RedirectToAction("Index");
            }

            itemLists.RemoveAll(x => x.ProductId == productId);

            HttpContext.Session.Set("CheckoutList", itemLists);

            if (itemLists.Count > 1)
                return RedirectToAction("ShopCheckout");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrder([FromBody] Order order)
        {
            await _elkRepository.CreateOrder(order);

            HttpContext.Session.Remove("CheckoutList");

            return Json(true, new JsonSerializerSettings());
        }
    }
}