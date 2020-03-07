using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.Controllers
{
    public class BookingController : Controller
    {
        
        private readonly ISugaringCentreAucklandElkRepository _elkRepository;

        public BookingController(ISugaringCentreAucklandElkRepository elkRepository)
        {
            _elkRepository = elkRepository;
        }
        
        public async Task<IActionResult> Index()
        {
            var bookingViewModel = new BookingViewModel();
            
            bookingViewModel.Categories = (await _elkRepository.GetServiceCategories()).ToList();
            bookingViewModel.Services = (await _elkRepository.GetServices()).ToList();
            
            return View(bookingViewModel);
        }
        
//        public async Task<IActionResult> ShowServiceTypesForService(int serviceId)
//        {
//            var bookingViewModel = new BookingViewModel();
//
//            bookingViewModel.MoveToServiceId = serviceId;
//            bookingViewModel.Categories = (await _elkRepository.GetServiceCategories()).ToList();
//            bookingViewModel.Services = (await _elkRepository.GetServices()).ToList();
//            
//            return View(bookingViewModel);
//        }
        
        public async Task<IActionResult> Service(int? serviceId, int? serviceTypeId)
        {
            var serviceTypeWithRecommended = new ServiceTypeWithRecommended();
            if (serviceTypeId == null || serviceId == null ||  serviceTypeId <= 0 )
            {
                RedirectToAction("Index");
            }

            serviceTypeWithRecommended.ServiceName = await _elkRepository.GetServiceCategoryTitleById(serviceId.Value);
            
            var servicesList = await _elkRepository.GetServiceForCategory(serviceId.Value);

            serviceTypeWithRecommended.ServicesToDisplay =
                servicesList.Single(x => x.ServiceId == serviceTypeId.Value);

            serviceTypeWithRecommended.RecommendedList =
                servicesList.Where(x => x.ServiceId != serviceTypeId.Value).Take(3).ToList();
            
            return View(serviceTypeWithRecommended);
        }

        public async Task<IActionResult> BookService(int? serviceTypeId)
        {
            if (serviceTypeId == null || serviceTypeId <= 0)
            {
                RedirectToAction("Index");
            }

            var viewModel = new BookingServiceViewModel();
            
            viewModel.Service = await _elkRepository.GetServiceById(serviceTypeId.Value);
            
            return View(viewModel);
        }
        
        public async Task<JsonResult> GetStaffForBooking (/*int serviceTypeId,*/ string searchStaffName)
        {
            if (searchStaffName == string.Empty)
            {
                return Json(null, new JsonSerializerSettings());
            }
            else
            {
                var services = await _elkRepository.GetStaff(searchStaffName);

                var modifiedData = services.Select(x => new
                {
                    id = x.StaffId,
                    text = (x.FirstName + ' ' + x.LastName)
                });

                return Json(modifiedData, new JsonSerializerSettings());
            }
        }

        public async Task<IActionResult> SaveBooking(BookingServiceViewModel bookingService)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}