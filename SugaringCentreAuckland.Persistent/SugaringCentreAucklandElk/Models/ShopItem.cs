using  System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;


namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ShopItem
    {
        public int ShopItemId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public decimal Price { get; set; }
        public byte[] ProductImg { get; set; }

    }
}
