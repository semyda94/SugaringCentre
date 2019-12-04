using System.Collections.Generic;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.ViewModel
{
    public class ShopViewModel
    {
        public List<ShopCategory> ShopCategories { get; set; }
        public List<ShopItem> ShopItems { get; set; }
        public int Sorting { get; set; } = 1;
        public int CategorySorting { get; set; } = -1;
    }
}
