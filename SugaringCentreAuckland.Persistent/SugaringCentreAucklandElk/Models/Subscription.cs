using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class Subscription
    {
        [Column("Subscription")]
        public int SubscriptionId { get; set; }
        public string Email { get; set; }
    }
}
