using System.Collections.Generic;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.ViewModel
{
    public class ServiceTypeWithRecommended
    {
        public string ServiceName { get; set; }
        public Service ServicesToDisplay { get; set; }
        
        public List<Service> RecommendedList { get; set; }
    }
}