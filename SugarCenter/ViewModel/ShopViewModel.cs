using System.Collections.Generic;
using System.Linq;
using SugarCenter.Classes;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.ViewModel
{
    public class ShopViewModel
    {
        public int PageIndex { get;  set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        
        public IEnumerable<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public int Sorting { get; set; } = 1;
        public int CategorySorting { get; set; } = -1;

        public List<Product> GetProductsForActivePage()
        {
            var count = this.Products.Count;
            var items = this.Products.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            return items;
        }
    }
}
