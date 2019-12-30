using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Service
    {
        public Service()
        {
            ServiceImage = new HashSet<ServiceImage>();
            ServiceStaff = new HashSet<ServiceStaff>();
        }

        [Column("Service")]
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<ServiceImage> ServiceImage { get; set; }
        public virtual ICollection<ServiceStaff> ServiceStaff { get; set; }
    }
}