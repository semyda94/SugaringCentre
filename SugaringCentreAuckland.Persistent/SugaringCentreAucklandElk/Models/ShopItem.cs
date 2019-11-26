using System;
using System.Collections.Generic;
using System.Text;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ShopItem
    {
        public int ShopItemId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public decimal Price { get; set; }

    }
}
