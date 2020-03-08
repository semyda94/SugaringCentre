using System;
using System.Collections.Generic;
using System.Globalization;
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
        
        public async Task<JsonResult> GetNotAvailableTime (int staffId, string dateString)
        {
            var dateToCheck = DateTime.ParseExact(dateString, "d MMMM, yyyy", CultureInfo.InvariantCulture);
            
            var bookings = await _elkRepository.GetBookingsForDate(staffId, dateString);

            var result = new List<int[]>();
            foreach (var booking in bookings)
            {
                result.Add(GetTimeInArray(booking.Time));
            }

            return Json(result, new JsonSerializerSettings());
        }

        private int[] GetTimeInArray(DateTime time)
        {
            return new []{time.Hour, time.Minute};
        }

        public async Task<IActionResult> SaveBooking(BookingServiceViewModel bookingService)
        {
            bookingService.Booking.Date = DateTime.ParseExact(bookingService.Booking.DateString, "d MMMM, yyyy", CultureInfo.InvariantCulture);
            bookingService.Booking.Time = DateTime.ParseExact(bookingService.Booking.TimeString, "h:mm tt", CultureInfo.InvariantCulture);

            await _elkRepository.CreateBooking(bookingService.Booking);
            return RedirectToAction("Index", "Home");
        }
    }
}