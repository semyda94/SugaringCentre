using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Leave
    {
        [Column("Leave")]
        public int LeaveId { get; set; }
        [Column("Staff")]
        public int StaffId { get; set; }
        public DateTime Date { get; set; }
        
        public virtual Staff StaffNavigation { get; set; }
    }
}