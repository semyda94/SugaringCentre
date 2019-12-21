using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using  System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            NavigationCategoryId = new Category();
        }

        [Column("Product")]
        public int ProductId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [Column("Description")]
        public string Desc { get; set; }
        public decimal Price { get; set; }
        /*public byte[] ProductImg { get; set; }*/
        [Column("Category")]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category NavigationCategoryId { get; set; }
    }
}
