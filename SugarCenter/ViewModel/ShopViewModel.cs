using System.Collections.Generic;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.ViewModel
{
    public class ShopViewModel
    {
        public List<ShopCategory> ShopCategories { get; set; }
        public List<ShopItem> ShopItems { get; set; }
    }
}
