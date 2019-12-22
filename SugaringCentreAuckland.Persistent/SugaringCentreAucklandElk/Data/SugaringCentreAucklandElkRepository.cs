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
            return await _DbContext.Categories/*.Include(c => c.Products)*/.ToListAsync();
        }
        
        public async Task<List<Category>> GetShopCategoriesForAc(string searchName)
        {
            return await (searchName == null ? _DbContext.Categories.ToListAsync() : _DbContext.Categories.Where(x => x.Name.Contains(searchName)).ToListAsync());
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _DbContext.Products.ToListAsync(); //.Include(t => t.NavigationCategoryId).ToListAsync();
        }

        public async Task<List<Product>> GetShopItemsForCategory(int? categoryId = -1, int? sorting = 1)
        {
            /*//return await _DbContext.Product.ToListAsync();
            var notSorted = categoryId == -1
                ? await _DbContext.Products.ToListAsync()
                : await _DbContext.Products.Where(i => i.CategoryId == categoryId).ToListAsync();

            switch (sorting)
            {
                case 3: return notSorted.OrderByDescending(x => x.Price).ToList();
                case 4: return notSorted.OrderBy(x => x.Price).ToList();
                default: return  notSorted;
            }*/
            return new List<Product>();
        }
        public async Task DeleteCategory(int categoryId)
        {
            var category = _DbContext.Categories.Single(c => c.CategoryId == categoryId);
            _DbContext.Categories.Remove(category);

            await _DbContext.SaveChangesAsync();
        }

        public async Task CreatCategory(string categoryName)
        {
            _DbContext.Categories.Add(new Category {Name = categoryName});
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateProduct(Product product)
        {
            _DbContext.Add(product);

            if (product.CategorySelected != null)
            {
                var splitedCategories = product.CategorySelected.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var productCategories = new List<ProductCategory>();
                
                foreach (var category in splitedCategories)
                {
                    productCategories.Add(new ProductCategory
                    {
                        CategoryId = Int32.Parse(category),
                        ProductId = product.ProductId
                    });
                }

                _DbContext.AddRange(productCategories);
            }

            await _DbContext.SaveChangesAsync();
        }

        public async Task DeleteProduct(int? productId)
        {
            var productCategories = await _DbContext.ProductCategory.Where(x => x.ProductId == productId).ToListAsync();
            var product = _DbContext.Products.Single(p => p.ProductId == productId);
            
            if (productCategories.Any())
                _DbContext.ProductCategory.RemoveRange(productCategories);
            _DbContext.Products.Remove(product);

            await _DbContext.SaveChangesAsync();
        }

        public async Task<Product> GetShopItem(int? productId)
        {
            var product = await _DbContext.Products/*.Include(i => i.NavigationCategoryId.Products)*/.FirstOrDefaultAsync(p => p.ProductId == productId);

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
