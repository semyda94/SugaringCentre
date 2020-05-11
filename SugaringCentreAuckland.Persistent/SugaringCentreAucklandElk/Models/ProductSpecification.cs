using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models
{
    [Table("ProductSpecification")]
    public partial class ProductSpecification
    {
        [Column("ProductSpecification")] 
        public int ProductSpecificationId { get; set; }
        [Column("Product")]
        public int ProductId { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        public string Details { get; set; }
        
    }
}