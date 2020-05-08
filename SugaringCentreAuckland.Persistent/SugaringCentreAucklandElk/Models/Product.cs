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
            ProductCategory = new HashSet<ProductCategory>();
            ProductImage = new HashSet<ProductImage>();
            ProductOrders = new HashSet<ProductOrder>();
        }
        
        [Key]
        [Column("Product")]
        public int ProductId { get; set; }
        public string Title { get; set; }
        [Column("Description")]
        public string Desc { get; set; }
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal Price { get; set; }
        [NotMapped] 
        public string CategorySelected { get; set; }
        [NotMapped]
        public List<IFormFile > ImagesToUpload { get; set; }

        [NotMapped] public int Qty { get; set; } = 1;

        [JsonIgnore] 
        public virtual ICollection<ProductCategory> ProductCategory { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
