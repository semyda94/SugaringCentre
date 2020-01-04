using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ServiceTypeImage
    {
        [Column("ServiceTypeImage")]
        public int ServiceTypeImageId { get; set; }
        [Column("ServiceType")]
        public int ServiceTypeId { get; set; }
        public byte[] Image { get; set; }

        public virtual ServiceType ServiceTypeNavigation { get; set; }
    }
}