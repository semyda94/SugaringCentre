using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Staff
    {
        public Staff()
        {
            StaffImage = new HashSet<StaffImage>();
            ServiceStaff = new HashSet<ServiceStaff>();
            Leaves = new HashSet<Leave>();
        }
        
        [Column("Staff")]
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime? Dob { get; set; }
        public string WorkingDayOfWeek { get; set; }
        [NotMapped]
        public List<IFormFile> ImagesToUpload { get; set; }
        
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<StaffImage> StaffImage { get; set; }
        public virtual ICollection<ServiceStaff> ServiceStaff { get; set; }
        public virtual ICollection<Leave> Leaves { get; set; }

        public IEnumerable<int> GetWorkingDaysIds()
        {
            return this.WorkingDayOfWeek.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        }
    }
}