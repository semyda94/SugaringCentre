using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class StaffImage
    {
        [Column("StaffImage")]
        public int StaffImageId { get; set; }
        [Column("Staff")]
        public int StaffId { get; set; }
        public byte[] Image { get; set; }
        
        public virtual Staff StaffNavigation { get; set; }
    }
}