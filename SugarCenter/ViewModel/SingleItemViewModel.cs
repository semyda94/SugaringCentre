using System.Collections.Generic;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugarCenter.ViewModel
{
    public class SingleItemViewModel
    {
        public Product Product;
        public IEnumerable<Product> RelatiedProducts;
    }
}