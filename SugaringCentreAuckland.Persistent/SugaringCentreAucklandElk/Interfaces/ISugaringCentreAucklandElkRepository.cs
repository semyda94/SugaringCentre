
using System.Collections.Generic;
using System.Threading.Tasks;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces
{
    public interface ISugaringCentreAucklandElkRepository
    {
        Task<List<Category>> GetShopCategories();
        Task<List<Category>> GetShopCategoriesForAc(string searchName);
        Task<List<Product>> GetProducts();
        Task<List<Product>> GetShopItemsForCategory(int? categoryId = -1, int? sorting = 1);
        Task DeleteCategory(int categoryId);
        Task CreatCategory(string categoryName);
        Task CreateProduct(Product product);

        Task DeleteProduct(int? productId);
        Task<Product> GetShopItem(int? productId);
        Task SubscribeForNews(string email);
    }
}
