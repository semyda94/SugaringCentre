
using System.Collections.Generic;
using System.Threading.Tasks;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces
{
    public interface ISugaringCentreAucklandElkRepository
    {
        Task<List<ShopCategory>> GetShopCategories();
        Task<List<ShopItem>> GetShoItems();
        Task<List<ShopItem>> GetShopItemsForCategory(int? categoryId = -1, int? sorting = 1);
        Task DeleteCategory(int categoryId);
        Task CreatCategory(string categoryName);
        Task CreateProduct(ShopItem product);

        Task DeleteProduct(int? productId);
        Task<ShopItem> GetShopItem(int? productId);
        Task SubscribeForNews(string email);
    }
}
