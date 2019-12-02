using System.Collections.Generic;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public virtual ICollection<ShopItem> ShopItems { get; set; }
    }
}
