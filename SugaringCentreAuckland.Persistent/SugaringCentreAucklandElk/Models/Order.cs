using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    [Table("Order")]
    public partial class Order
    {
        public Order()
        {
            ProductOrders = new HashSet<ProductOrder>();
            OrderItems = new HashSet<OrderItem>();
        }
        
        [Column("Order")]
        public int OrderId { get; set; }

        public string Client { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string ExternalId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}