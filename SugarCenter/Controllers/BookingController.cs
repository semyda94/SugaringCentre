using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SugarCenter.ViewModel;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;

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
    }
}