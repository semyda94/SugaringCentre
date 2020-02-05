using System.Linq;
using System.Threading.Tasks;
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
        
        // GET
        public async Task<IActionResult> Index()
        {
            var bookingViewModel = new BookingViewModel();
            bookingViewModel.Services = (await _elkRepository.GetServices()).ToList();
            bookingViewModel.ServiceTypes = (await _elkRepository.GetServiceTypes()).ToList();
            
            return View(bookingViewModel);
        }
        
        public async Task<IActionResult> Service(int? serviceId, int? serviceTypeId)
        {
            var serviceTypeWithRecommended = new ServiceTypeWithRecommended();
            if (serviceTypeId == null || serviceId == null ||  serviceTypeId <= 0 )
            {
                RedirectToAction("Index");
            }

            var servicesList = await _elkRepository.GetServiceTypesForService(serviceId.Value);

            serviceTypeWithRecommended.ServiceTypeToDisplay =
                servicesList.Single(x => x.ServiceTypeId == serviceTypeId.Value);

            serviceTypeWithRecommended.RecommendedList =
                servicesList.Where(x => x.ServiceTypeId != serviceTypeId.Value).Take(3).ToList();
            
            return View(serviceTypeWithRecommended);
        }

        public async Task<IActionResult> BookService(int? serviceTypeId)
        {
            if (serviceTypeId == null || serviceTypeId <= 0)
            {
                RedirectToAction("Index");
            }

            var viewModel = new BookingServiceViewModel();
            
            viewModel.ServiceType = await _elkRepository.GetServiceType(serviceTypeId.Value);
            
            return View(viewModel);
        }
        
        public async Task<JsonResult> GetStaffForBooking (/*int serviceTypeId,*/ string searchStaffName)
        {
            var services = await _elkRepository.GetStaff(searchStaffName);
            
            var modifiedData = services.Select(x => new
            {
                id = x.StaffId,
                text = (x.FirstName + ' ' +  x.LastName)
            });
            return Json(modifiedData, new JsonSerializerSettings());
        }

        public async Task<IActionResult> SaveBooking(BookingServiceViewModel bookingService)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}