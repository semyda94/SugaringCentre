﻿using System;
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
            var categories = _elkRepository.GetListOfCategories().GetAwaiter().GetResult();

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
        public IActionResult AddCategory(string newCategory)
        {
            _elkRepository.CreatCategory(newCategory).GetAwaiter().GetResult();
            return RedirectToAction("Category");
        }

        public async Task<JsonResult> GetCategory(string searchCategoryName)
        {
            var category = await _elkRepository.SearchCategoryByTitle(searchCategoryName);
            var modifiedData = category.Select(x => new
            {
                id = x.CategoryId,
                text = x.Name
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }

        #endregion

        #region Products

        //TODO : Implemet paginated for admin console
        public async Task<IActionResult> Products()
        {
            var viewMovel = new ShopViewModel();

            viewMovel.Categories = await _elkRepository.GetListOfCategories();
            viewMovel.Products = await _elkRepository.GetListOfProducts();

            return View(viewMovel);
        }
        
        public async Task<IActionResult> DeleteProduct(int? productId)
        {
            if (productId != null)
            {
                await _elkRepository.DeleteProduct(productId);
            }

            return RedirectToAction("Products");
        }

        public async Task<IActionResult> ProductConfiguration(int? productId)
        {
            return View(productId == null ? new Product{ProductId = -1} : await _elkRepository.GetProductById(productId));
        }
        
        public JsonResult SaveCategorySelection(string categoryIds)
        {
            return Json(categoryIds, new JsonSerializerSettings());
        }
        
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> SaveProduct(Product product)
        {
            if (product.ProductId <= 0 )
            {
                await _elkRepository.CreateProduct(product);
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
        
        public async Task<JsonResult> GetStaff (string searchStaffName)
        {
            var services = await _elkRepository.GetStaff(searchStaffName);
            
            var modifiedData = services.Select(x => new
            {
                id = x.StaffId,
                text = (x.FirstName + ' ' +  x.LastName)
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }
        
        public async Task<JsonResult> GetStaffForService (int serviceId, string searchStaffName)
        {
            var services = await _elkRepository.GetStaff(searchStaffName);
            
            
            var modifiedData = services.Select(x => new
            {
                id = x.StaffId,
                text = (x.FirstName + ' ' +  x.LastName)
            });
            return Json(modifiedData, new JsonSerializerSettings());
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
            return View(staffId == null ? new Staff() : await _elkRepository.GetStaffWithLeaves(staffId.Value));
        }

        public async Task<IActionResult> SaveStaff(Staff staff)
        {
            if (staff.StaffId <= 0 )
            {
                await _elkRepository.CreateStaff(staff);
            }
            else
            {
                await _elkRepository.UpdateStaff(staff);
            }
            
            return RedirectToAction("Staff");
        }

        #endregion

        #region Leave

        public async Task<IActionResult> DeleteLeave(int staffId, int leaveId)
        {
            await _elkRepository.DeleteLeave(leaveId);
            return RedirectToAction("StaffConfiguration", new {staffId = staffId});
        }

        public async Task<IActionResult> CreateLeave(int staffId, DateTime leaveDate, string leaveReason)
        {
            await _elkRepository.CreateLeave(staffId, leaveDate, leaveReason);
            return RedirectToAction("StaffConfiguration", new {staffId = staffId});
        }
        #endregion

        #region ServiceCategories

        public async Task<IActionResult> ServiceCategories()
        {
            var services = await _elkRepository.GetServiceCategoriesWithRelatedServices();
            return View(services);
        }

        public async Task<IActionResult> DeleteServiceCategory(int? serviceCategoryId)
        {
            if (serviceCategoryId != null)
                await _elkRepository.DeleteServiceCategory(serviceCategoryId.Value);
                
            return RedirectToAction("ServiceCategories");
        }

        public async Task<IActionResult> ServiceCategoryConfiguration(int? serviceCategoryId)
        {
            return View(serviceCategoryId == null
                ? new ServiceCategory
                {
                    ServiceCategoryId = -1
                }
                : await _elkRepository.GetServiceCategoryById(serviceCategoryId.Value));
        }
        
        public async Task<IActionResult> SaveServiceCategory(ServiceCategory serviceCategory)
        {
            if (serviceCategory.ServiceCategoryId <= 0 )
            {
                await _elkRepository.CreateServiceCategory(serviceCategory);
            }
            else
            {
                await _elkRepository.UpdateServiceCategory(serviceCategory);
            }
            
            return RedirectToAction("ServiceCategories");
        }
        
        public async Task<JsonResult> GetServiceCategory(string searchCategoryName)
        {
            var category = await _elkRepository.SearchServiceCategoryByTitle(searchCategoryName);
            var modifiedData = category.Select(x => new
            {
                id = x.ServiceCategoryId,
                text = x.Title
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }

        #endregion

        #region Services

        public async Task<IActionResult> Services()
        {
            var services = await _elkRepository.GetServices();
            return View(services);
        }
        
        public async Task<JsonResult> GetServices (string searchServiceName)
        {
            var services = await _elkRepository.GetServiceBySearchTitle(searchServiceName);
            
            var modifiedData = services.Select(x => new
            {
                id = x.ServiceId,
                text = x.Title
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }

        public async Task<IActionResult> DeleteService(int? serviceId)
        {
            if (serviceId != null)
                await _elkRepository.DeleteService(serviceId.Value);
            return RedirectToAction("Services");
        }

        public async Task<IActionResult> ServiceConfiguration(int? serviceId)
        {
            return View(serviceId == null
                ? new Service
                {
                    ServiceId = -1
                }
                : await _elkRepository.GetServiceById(serviceId.Value));
        }
        
        public async Task<IActionResult> SaveService(Service service)
        {
            if (service.ServiceId <= 0 )
            {
                await _elkRepository.CreateService(service);
            }
            else
            {
                await _elkRepository.UpdateService(service);
            }
            
            return RedirectToAction("Services");
        }

        #endregion
        
        #region Booking

        public async Task<IActionResult> Bookings()
        {
            var staff = await _elkRepository.GetStaffList();
            return View(staff);
        }

        public IActionResult BookingConfiguration(int bookingId)
        {
            var booking = _elkRepository.GetBooking(bookingId);
            return View(booking);
        }
        
        public JsonResult BookingsGetDataForStaff(int staffId)
        {
            var bookings = _elkRepository.GetBookingsForStaff(staffId).GetAwaiter().GetResult();

            var result = bookings.Select(x => new
            {
                title = $"{x.ServiceNavigation.Title}\n{x.FirstName} {x.LastName}", 
                start = new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, x.Time.Hour, x.Time.Minute, 0),
                url = Url.Action("BookingConfiguration", new {bookingId = x.BookingId})
            });
            
            return Json(result, new JsonSerializerSettings());
        }

        #endregion
        
    }
}