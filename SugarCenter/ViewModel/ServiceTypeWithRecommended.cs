using System.Collections.Generic;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.ViewModel
{
    public class ServiceTypeWithRecommended
    {
        public ServiceType ServiceTypeToDisplay { get; set; }
        
        public List<ServiceType> RecommendedList { get; set; }
    }
}