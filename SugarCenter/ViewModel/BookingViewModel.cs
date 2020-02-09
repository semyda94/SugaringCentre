using System.Collections.Generic;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.ViewModel
{
    public class BookingViewModel
    {
        public int MoveToServiceId { get; set; }
        public List<Service> Services;
        public List<ServiceType> ServiceTypes;
    }
}