using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Booking
    {
        [Column("Booking")]
        public int BookingId { get; set; }
        [Column("Service")]
        public int ServiceId { get; set; }
        [Column("Staff")]
        public int StaffId { get; set; }
        [Column("Client")]
        public int? ClientId { get; set; }
        public DateTime DateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }

        public virtual Service ServiceNavigation { get; set; }
        public virtual Staff StaffNavigation { get; set; }
    }
}