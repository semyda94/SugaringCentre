using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            ServiceTypeStaff = new HashSet<ServiceTypeStaff>();
            ServiceTypeImage = new HashSet<ServiceTypeImage>();
        }
        [Column("ServiceType")]
        public int ServiceTypeId { get; set; }
        [Column("Service")]
        public int ServiceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        
        [NotMapped]
        public List<IFormFile> ImagesToUpload { get; set; }
        [NotMapped]
        public string SelectedStaff { get; set; }
        
        public virtual ICollection<ServiceTypeStaff> ServiceTypeStaff { get; set; }
        public virtual ICollection<ServiceTypeImage> ServiceTypeImage { get; set; }
    }
}