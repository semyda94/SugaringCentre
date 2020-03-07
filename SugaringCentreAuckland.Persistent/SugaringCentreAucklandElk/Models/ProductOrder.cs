using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public class ProductOrder
    {
        [Column("ProductOrder")]
        public int ProductOrderId { get; set; }
        [Column("Product")]
        public int ProductId { get; set; }
        [Column("Order")]
        public int OrderId { get; set; }
        
        public virtual Order OrderNavigation { get; set; }
        public virtual Product ProductNavigation { get; set; }
    }
}