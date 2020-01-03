using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Service
    {
        public Service()
        {
            ServiceImage = new HashSet<ServiceImage>();
        }

        [Column("Service")]
        public int ServiceId { get; set; }
        public string Name { get; set; }
        [Column("Description")]
        public string Desc { get; set; }

        [NotMapped] public List<IFormFile> ImagesToUpload { get; set; }

        public virtual ICollection<ServiceImage> ServiceImage { get; set; }
        public virtual ICollection<ServiceType> ServiceType { get; set; }
    }
}