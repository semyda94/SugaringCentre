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

        #region Service

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

        public async Task<IActionResult> ServiceConfiguration(int? serviceId)
        {
            return View(serviceId == null ? new Service{ServiceId = -1} : await _elkRepository.GetService(serviceId.Value));
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

        #region Service

        public async Task<IActionResult> ServiceType()
        {
            var servicesTypes = await _elkRepository.GetServiceTypes();
            return View(servicesTypes);
        }
        
        public async Task<JsonResult> GetServices (string searchServiceName)
        {
            var services = await _elkRepository.GetServiceTypes(searchServiceName);
            
            var modifiedData = services.Select(x => new
            {
                id = x.ServiceId,
                text = x.Name
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }

        public async Task<IActionResult> DeleteServiceType(int? serviceTypeId)
        {
            if (serviceTypeId != null)
                await _elkRepository.DeleteServiceType(serviceTypeId.Value);
                
            return RedirectToAction("ServiceType");
        }

        public async Task<IActionResult> ServiceTypeConfiguration(int? serviceTypeId)
        {
            return View(serviceTypeId == null ? new ServiceType{ServiceTypeId = -1} : await _elkRepository.GetServiceType(serviceTypeId.Value));
        }
        
        public async Task<IActionResult> SaveServiceType(ServiceType serviceType)
        {
            if (serviceType.ServiceTypeId <= 0 )
            {
                await _elkRepository.CreateServiceType(serviceType);
            }
            else
            {
                await _elkRepository.UpdateServiceType(serviceType);
            }
            
            return RedirectToAction("ServiceType");
        }

        #endregion
        
        
    }
}