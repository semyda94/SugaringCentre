using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SugarCenter.Classes;
using SugarCenter.Models;
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

        private string _encryptionString = "Sugaring";

        public AdminConsoleController(ISugaringCentreAucklandElkRepository sugaringCentreAucklandElkRepository,
            IHostingEnvironment hostingEnvironment)
        {
            _elkRepository = sugaringCentreAucklandElkRepository;
            _hostingEnvironment = hostingEnvironment;
        }


        #region Statistics
        
        [Authorize]
        public IActionResult Index()
        {
            var vm = new StatisticViewMovel();
            // vm.OrdersNumber = _elkRepository.GetOrdersNumber();
            // vm.OrdersValue = _elkRepository.GetOrdersValue();
            vm.BookingsNumber = _elkRepository.GetBookingsNumber();
            return View(vm);
        }

        public JsonResult GetTopBookingsPerMaster()
        {
            var bookingsPerMaster = _elkRepository.GetTopBookingsPerMaster();
            
            return Json(bookingsPerMaster, new JsonSerializerSettings());
        }
        
        public JsonResult GetTopBookingsPerService()
        {
            var bookingsPerService = _elkRepository.GetTopBookingsPerService();
            
            return Json(bookingsPerService, new JsonSerializerSettings());
        }
        
        #endregion
        
        #region Login

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(LoginModel login)
        {
            var staff = _elkRepository.ValidateLoginModel(login.Username,
                Encrypt.EncryptString(login.Password, "Sugaring"));

            if (staff == null)
                return RedirectToAction("Login", new LoginModel());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, staff.FirstName),
                new Claim(ClaimTypes.Surname, staff.LastName),
                new Claim(ClaimTypes.Role, staff.Title),
                new Claim(ClaimTypes.Sid, staff.StaffId.ToString())
            };

            var userIdentity = new ClaimsIdentity(claims, "login");

            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(principal);

            //Just redirect to our index after logging in. 
            return RedirectToAction("Index");
        }

        public async Task<JsonResult> LoggedUserInfo()
        {
            var claims = HttpContext.User.Claims.Select(c => new {c.Type, c.Value});

            var result = new List<(string type, string value)>();
            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.Name:
                        result.Add(("Name",claim.Value));
                        break;
                    case ClaimTypes.Surname:
                        result.Add(("LastName", claim.Value));
                        break;
                    case ClaimTypes.Role:
                        result.Add(("Title", claim.Value));
                        break;
                    case ClaimTypes.Sid:
                        result.Add(("Id", claim.Value));
                        break;
                    default:
                        break;
                }
            }

            return Json(result, new JsonSerializerSettings());
        }

    #endregion
        
        #region Category
        [Authorize]
        public IActionResult Category()
        {
            var categories = _elkRepository.GetListOfCategories().GetAwaiter().GetResult();

            return View(categories);
        }

        [Authorize]
        public IActionResult DeleteCategory(int? id)
        {
            if (id.HasValue)
            {
                _elkRepository.DeleteCategory(id.Value);
            }
            return RedirectToAction("Category");
        }

        [Authorize]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult AddCategory(string newCategory)
        {
            _elkRepository.CreatCategory(newCategory).GetAwaiter().GetResult();
            return RedirectToAction("Category");
        }

        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Products()
        {
            var viewMovel = new ShopViewModel();

            viewMovel.Categories = await _elkRepository.GetListOfCategories();
            viewMovel.Products = await _elkRepository.GetListOfProducts();

            return View(viewMovel);
        }
        
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int? productId)
        {
            if (productId != null)
            {
                await _elkRepository.DeleteProduct(productId);
            }

            return RedirectToAction("Products");
        }

        [Authorize]
        public async Task<IActionResult> ProductConfiguration(int? productId)
        {
            return View(productId == null ? new Product{ProductId = -1} : await _elkRepository.GetProductById(productId));
        }
        
        [Authorize]
        public JsonResult SaveCategorySelection(string categoryIds)
        {
            return Json(categoryIds, new JsonSerializerSettings());
        }
        
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Authorize]
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
        
        [Authorize]
        public async Task<IActionResult> Staff()
        {
            var staffList = await _elkRepository.GetStaffList();
            return View(staffList);
        }
        
        [Authorize]
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
        
        [Authorize]
        public async Task<JsonResult> GetStaffForService (int serviceId, string searchStaffName)
        {
            var staff = await _elkRepository.GetStaffForService(serviceId);

            var modifiedData = staff.Select(x => new
            {
                id = x.StaffId,
                text = (x.FirstName + ' ' +  x.LastName)
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }

        [Authorize]
        public async Task<IActionResult> DeleteStaff(int? staffId)
        {
            if (staffId != null)
            {
               await _elkRepository.DeleteStaff(staffId.Value);
            }

            return RedirectToAction("Staff");
        }
        
        [Authorize]
        public async Task<IActionResult> StaffConfiguration(int? staffId)
        {
            return View(staffId == null ? new Staff() : await _elkRepository.GetStaffWithLeaves(staffId.Value));
        }

        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> DeleteLeave(int staffId, int leaveId)
        {
            await _elkRepository.DeleteLeave(leaveId);
            return RedirectToAction("StaffConfiguration", new {staffId = staffId});
        }

        [Authorize]
        public async Task<IActionResult> CreateLeave(int staffId, DateTime leaveDate, string leaveReason)
        {
            await _elkRepository.CreateLeave(staffId, leaveDate, leaveReason);
            return RedirectToAction("StaffConfiguration", new {staffId = staffId});
        }
        #endregion

        #region ServiceCategories

        [Authorize]
        public async Task<IActionResult> ServiceCategories()
        {
            var services = await _elkRepository.GetServiceCategoriesWithRelatedServices();
            return View(services);
        }

        [Authorize]
        public async Task<IActionResult> DeleteServiceCategory(int? serviceCategoryId)
        {
            if (serviceCategoryId != null)
                await _elkRepository.DeleteServiceCategory(serviceCategoryId.Value);
                
            return RedirectToAction("ServiceCategories");
        }

        [Authorize]
        public async Task<IActionResult> ServiceCategoryConfiguration(int? serviceCategoryId)
        {
            return View(serviceCategoryId == null
                ? new ServiceCategory()
                : await _elkRepository.GetServiceCategoryById(serviceCategoryId.Value));
        }
        
        [Authorize]
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
        
        [Authorize]
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

        [Authorize]
        public async Task<IActionResult> Services()
        {
            var services = await _elkRepository.GetServices();
            return View(services);
        }
        
        [Authorize]
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

        [Authorize]
        public async Task<IActionResult> DeleteService(int? serviceId)
        {
            if (serviceId != null)
                await _elkRepository.DeleteService(serviceId.Value);
            return RedirectToAction("Services");
        }

        [Authorize]
        public async Task<IActionResult> ServiceConfiguration(int? serviceId)
        {
            return View(serviceId == null
                ? new Service()
                : await _elkRepository.GetServiceById(serviceId.Value));
        }
        
        [Authorize]
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

        [Authorize]
        public async Task<IActionResult> Bookings()
        {
            var staff = await _elkRepository.GetStaffList();
            return View(staff);
        }

        [Authorize]
        public IActionResult BookingConfiguration(int? bookingId)
        {
            return View(bookingId == null
                ? new Booking()
                : _elkRepository.GetBooking(bookingId.Value));
        }
        
        [Authorize]
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
        
        [Authorize]
        public async Task<IActionResult> SaveBooking(Booking booking)
        {
            if (booking.BookingId > 0)
            {
                await _elkRepository.UpdateBooking(booking);
            }
            else
            {
                await _elkRepository.CreateBooking(booking);
            }
            return RedirectToAction("Bookings");
        }

        [Authorize]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            if (bookingId > 0)
                await _elkRepository.DeleteBooking(bookingId);
            
            return RedirectToAction("Bookings");
        }

        #endregion
        
    }
}