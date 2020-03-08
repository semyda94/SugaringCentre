using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    [Table("Booking")]
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
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }

        [NotMapped]
        public string DateString { get; set; }
//        {
//            get => "";
//            set => Date = DateTime.ParseExact(value, "{0:d MMMM, yyyy}", CultureInfo.InvariantCulture);
//        }

        [NotMapped]
        public string TimeString { get; set; }
//        {
//            get => "";
//            set => Time = DateTime.ParseExact(value, "{0:t}", CultureInfo.InvariantCulture);
//        }

        public virtual Service ServiceNavigation { get; set; }
        public virtual Staff StaffNavigation { get; set; }
    }
}