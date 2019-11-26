using System.Collections.Generic;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ShopCategory
    {

        public int ShopCategoryId { get; set; }
        public string Name { get; set; }

        //public virtual ICollection<ShopItem> ShopItem { get; set; }
    }
}
