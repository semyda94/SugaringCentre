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
            
            bookingViewModel.Categories = (await _elkRepository.GetServiceCategoriesWithRelatedServices()).ToList();
            bookingViewModel.Services = (await _elkRepository.GetServices()).ToList();
            
            return View(bookingViewModel);
        }
        
//        public async Task<IActionResult> ShowServiceTypesForService(int serviceId)
//        {
//            var bookingViewModel = new BookingViewModel();
//
//            bookingViewModel.MoveToServiceId = serviceId;
//            bookingViewModel.Categories = (await _elkRepository.GetServiceCategoriesWithRelatedServices()).ToList();
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
        
        public async Task<JsonResult> GetStaffForBooking (int serviceId, string searchStaffName)
        {
            var staffForService = await _elkRepository.GetStaffForService(serviceId);

            if (string.IsNullOrEmpty(searchStaffName))
            {
                var result = staffForService.Select(x => new
                {
                    id = x.StaffId,
                    text = (x.FirstName + ' ' + x.LastName)
                });
                return Json(result, new JsonSerializerSettings());
            }
            else
            {
                var staffSortedByName =
                    staffForService.Where(x => (x.FirstName + ' ' + x.LastName).ToLowerInvariant()
                        .Contains(searchStaffName.ToLowerInvariant()));

                var result = staffSortedByName.Select(x => new
                {
                    id = x.StaffId,
                    text = (x.FirstName + ' ' + x.LastName)
                });

                return Json(result, new JsonSerializerSettings());
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

        public async Task<JsonResult> GetNotAvailableDays(int staffId)
        {
            var daysOfWeek = new List<int>(new []{1,2,3,4,5,6,7});
            
            var staff = await _elkRepository.GetStaff(staffId);

            var workingDays = staff.GetWorkingDaysIds();

            var workingDaysToExclude = daysOfWeek.Where(x => !workingDays.Contains(x));
            
            return Json(workingDaysToExclude, new JsonSerializerSettings());
        }
        
        public async Task<JsonResult> GetLeaveDays(int staffId)
        {
            var leaves = await _elkRepository.GetLeavesForStaff(staffId);

            var result = leaves.Where(x => x.Date >= DateTime.Today).Select(x => new int []
            {
                x.Date.Year,
                x.Date.Month - 1,
                x.Date.Day
            });

            return Json(result, new JsonSerializerSettings());
        }

        private int[] GetTimeInArray(DateTime time)
        {
            return new []{time.Hour, time.Minute};
        }

        public async Task<IActionResult> SaveBooking(Booking booking)
        {
            booking.Date = DateTime.ParseExact(booking.DateString, "d MMMM, yyyy", CultureInfo.InvariantCulture);
            booking.Time = DateTime.ParseExact(booking.TimeString, "h:mm tt", CultureInfo.InvariantCulture);

            await _elkRepository.CreateBooking(booking);
            return RedirectToAction("Index", "Home");
        }
    }
}