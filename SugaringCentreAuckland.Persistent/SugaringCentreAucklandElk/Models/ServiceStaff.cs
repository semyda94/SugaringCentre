using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public class ServiceStaff
    {
        [Column("ServiceStaff")]
        public int ServiceStaffId { get; set; }
        [Column("Service")]
        public int ServiceId { get; set; }
        [Column("Staff")]
        public int StaffId { get; set; }

        public virtual Service ServiceNavigation { get; set; }
        public virtual Staff StaffNavigation { get; set; }
    }
}