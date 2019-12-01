using System.Collections.Generic;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ShopCategory
    {
        public ShopCategory()
        {
            ShopItems = new HashSet<ShopItem>();
        }

        public int ShopCategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ShopItem> ShopItems { get; set; }
    }
}
