using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ProductImage
    {
        [Column("ProductImage")]
        public int ProductImageId { get; set; }
        [Column("Product")]
        public int ProductId { get; set; }
        public byte[] Image { get; set; }
        [JsonIgnore]
        public virtual Product ProductNavigation { get; set; }
    }
}