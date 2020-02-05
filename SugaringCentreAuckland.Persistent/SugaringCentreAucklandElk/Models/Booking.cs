using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Booking
    {
        [Column("Booking")]
        public int BookingId { get; set; }
        [Column("ServiceType")]
        public int ServiceTypeId { get; set; }
        [Column("Staff")]
        public int StaffId { get; set; }
        public DateTime DateTime { get; set; }
        [Required] 
        public string Name { get; set; }
        [Required] 
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }

        [NotMapped]
        public DateTime? Date { get; set; }
        [NotMapped]
        public DateTime? Time { get; set; }
    }
}