using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ServiceStaff
    {
        [Column("ServiceStaff")]
        public int ServiceStaffId { get; set; }
        public int Service { get; set; }
        public int Staff { get; set; }

        public virtual Service ServiceNavigation { get; set; }
        public virtual Staff StaffNavigation { get; set; }
    }
}