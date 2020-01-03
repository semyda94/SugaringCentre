using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ServiceTypeStaff
    {
        [Column("ServiceTypeStaff")]
        public int ServiceTypeStaffId { get; set; }
        public int ServiceType { get; set; }
        public int Staff { get; set; }

        public virtual ServiceType ServiceTypeNavigation { get; set; }
        public virtual Staff StaffNavigation { get; set; }
    }
}