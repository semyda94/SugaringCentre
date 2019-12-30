using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ServiceImage
    {
        [Column("ServiceImage")]
        public int ServiceImageId { get; set; }
        public int Service { get; set; }
        public byte[] Image { get; set; }

        public virtual Service ServiceNavigation { get; set; }
    }
}