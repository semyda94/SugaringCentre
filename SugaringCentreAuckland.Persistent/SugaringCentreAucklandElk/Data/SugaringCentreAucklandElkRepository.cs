using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Data
{
    public class SugaringCentreAucklandElkRepository : ISugaringCentreAucklandElkRepository
    {
        private readonly SugaringCentreAucklandElkContext _DbContext;

        public SugaringCentreAucklandElkRepository(SugaringCentreAucklandElkContext ElkContext) =>
            _DbContext = ElkContext;

        public async Task<List<ShopCategory>> GetShopCategories()
        {
            return await _DbContext.ShopCategory.ToListAsync();
        }

        public async Task<List<ShopItem>> GetShoItems()
        {
            return await _DbContext.ShopItem.ToListAsync();
        }

        public async Task<List<ShopItem>> GetShopItemsForCategory(int? categoryId = -1)
        {
            return categoryId == -1
                ? await _DbContext.ShopItem.ToListAsync()
                : await _DbContext.ShopItem.Where(i => i.ShopCategoryId == categoryId).ToListAsync();
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category = _DbContext.ShopCategory.Single(c => c.ShopCategoryId == categoryId);
            _DbContext.ShopCategory.Remove(category);

            await _DbContext.SaveChangesAsync();
        }

        public async Task CreatCategory(string categoryName)
        {
            _DbContext.ShopCategory.Add(new ShopCategory {Name = categoryName});
            await _DbContext.SaveChangesAsync();
        }
    }
}
