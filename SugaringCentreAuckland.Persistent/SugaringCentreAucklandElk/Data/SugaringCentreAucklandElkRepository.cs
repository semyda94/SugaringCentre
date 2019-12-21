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

        public async Task<List<Category>> GetShopCategories()
        {
            return await _DbContext.ShopCategory.Include(c => c.Products).ToListAsync();
        }

        public async Task<List<Product>> GetShoItems()
        {
            return await _DbContext.ShopItem.Include(t => t.NavigationCategoryId).ToListAsync();
        }

        public async Task<List<Product>> GetShopItemsForCategory(int? categoryId = -1, int? sorting = 1)
        {
            //return await _DbContext.Product.ToListAsync();
            var notSorted = categoryId == -1
                ? await _DbContext.ShopItem.ToListAsync()
                : await _DbContext.ShopItem.Where(i => i.CategoryId == categoryId).ToListAsync();

            switch (sorting)
            {
                case 3: return notSorted.OrderByDescending(x => x.Price).ToList();
                case 4: return notSorted.OrderBy(x => x.Price).ToList();
                default: return  notSorted;
            }
        }
        public async Task DeleteCategory(int categoryId)
        {
            var category = _DbContext.ShopCategory.Single(c => c.CategoryId == categoryId);
            _DbContext.ShopCategory.Remove(category);

            await _DbContext.SaveChangesAsync();
        }

        public async Task CreatCategory(string categoryName)
        {
            _DbContext.ShopCategory.Add(new Category {Name = categoryName});
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateProduct(Product product)
        {
            _DbContext.Add(product);
            await _DbContext.SaveChangesAsync();
        }

        public async Task DeleteProduct(int? productId)
        {
            var product = _DbContext.ShopItem.Single(p => p.ProductId == productId);
            _DbContext.ShopItem.Remove(product);

            await _DbContext.SaveChangesAsync();
        }

        public async Task<Product> GetShopItem(int? productId)
        {
            var product = await _DbContext.ShopItem.Include(i => i.NavigationCategoryId.Products).FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return new Product{ ProductId = -1};
            }
            else
            {
                return product;
            }
        }

        public async Task SubscribeForNews(string email)
        {
            _DbContext.Subscription.Add(new Subscription {Email = email});
            await _DbContext.SaveChangesAsync();
        }
    }
}
