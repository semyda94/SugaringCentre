using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public class ServiceCategory
    {
        public ServiceCategory()
        {
            Services = new HashSet<Service>();
        }
        [Column("SerrviceCategory")]
        public int ServiceCategoryId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}