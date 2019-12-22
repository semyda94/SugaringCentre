using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            ProductCategory = new HashSet<ProductCategory>();
        }
        
        [Column("Category")]
        public int CategoryId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        
        public virtual ICollection<ProductCategory> ProductCategory { get; set; }
    }
}
