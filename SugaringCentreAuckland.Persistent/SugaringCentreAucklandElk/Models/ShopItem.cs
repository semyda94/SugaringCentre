using System.Collections.Generic;
using  System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ShopItem
    {
        //public ShopItem()
        //{
        //    NavigationShopCategoryId = new HashSet<ShopCategory>();
        //}

        public int ShopItemId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public decimal Price { get; set; }
        public byte[] ProductImg { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual ShopCategory NavigationShopCategoryId { get; set; }
    }
}
