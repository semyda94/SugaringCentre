using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    public partial class ProductCategory
    {
        [Column("ProductCategory")]
        public int ProductCategoryId { get; set; }
        [Column("Product")]
        public int ProductId { get; set; }
        [Column("Category")]
        public int CategoryId { get; set; }

        public virtual Category CategoryNavigation { get; set; }
        public virtual Product ProductNavigation { get; set; }
    }
}