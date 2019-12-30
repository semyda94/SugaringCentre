﻿using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<List<Product>> GetproductsForCategory(int? categoryId = -1, int? sorting = 1)
        {
            //return await _DbContext.Product.ToListAsync();
            var notSorted = categoryId == -1
                ? await _DbContext.Products.Include(x => x.ProductImage).ToListAsync()
                : await _DbContext.ProductCategory.Include(x => x.ProductNavigation).ThenInclude(x => x.ProductImage).Where(i => i.CategoryId == categoryId).Select(x => x.ProductNavigation).ToListAsync();

            switch (sorting)
            {
                case 3: return notSorted.OrderByDescending(x => x.Price).ToList();
                case 4: return notSorted.OrderBy(x => x.Price).ToList();
                default: return  notSorted;
            }
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

        public async Task CreateProduct(Product product, string projectWebRootPath)
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

            if (product.ImagesToUpload.Any())
            {
                var productImagesToSave = new List<ProductImage>();
                foreach (var image in product.ImagesToUpload)
                {
                    if (image.Length > 0)
                    {
                       
                        using (var stream = new MemoryStream())
                        {
                            await image.CopyToAsync(stream);
                            productImagesToSave.Add(new ProductImage
                            {
                                ProductId = product.ProductId,
                                Image = stream.ToArray()
                            });
                        }
                    }
                }
                
                _DbContext.ProductImage.AddRange(productImagesToSave);
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

        public async Task UpdateProduct(Product product)
        {
            _DbContext.Products.Update(product);
            await _DbContext.SaveChangesAsync();
        }

        public async Task<Product> GetShopItem(int? productId)
        {
            var product = await _DbContext.Products.Include(i => i.ProductImage).FirstOrDefaultAsync(p => p.ProductId == productId);

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

        #region Staff

        public async Task<IEnumerable<Staff>> GetStaffList()
        {
            return await _DbContext.Staff.ToListAsync();
        }

        public async Task<Staff> GetStaff(int staffId)
        {
            return await _DbContext.Staff.Where(x => x.StaffId == staffId).SingleOrDefaultAsync();
        }

        public async Task DeleteStaff(int staffId)
        {
            var staff = _DbContext.Staff.SingleOrDefault(x => x.StaffId == staffId);

            if (staff != null)
            {
                _DbContext.Staff.Remove(staff);
                await _DbContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
