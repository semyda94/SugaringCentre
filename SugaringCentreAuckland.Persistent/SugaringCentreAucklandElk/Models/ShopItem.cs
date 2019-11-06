using System;
using System.Collections.Generic;
using System.Text;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ShopItem
    {
        public int ShopItemId { get; set; }
        public int? ShopCategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public virtual ShopCategory ShopCategory { get; set; }
    }
}
