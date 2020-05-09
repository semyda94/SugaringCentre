using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    [Table("OrderItem")]
    public partial class OrderItem
    {
        [Column("OrderItem")]
        public int OderItemId { get; set; }
        [Column("Order")]
        public int OrderId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        [Column("Quantity")]
        public int Qty { get; set; }
        
        public virtual Order OrderNavigation { get; set; }
    }
}