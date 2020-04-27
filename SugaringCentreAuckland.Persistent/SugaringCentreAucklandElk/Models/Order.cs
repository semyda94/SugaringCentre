using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Order
    {
        public Order()
        {
            ProductOrders = new HashSet<ProductOrder>();
        }
        [Column("Order")]
        public int OrderId { get; set; }

        public string Client { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}