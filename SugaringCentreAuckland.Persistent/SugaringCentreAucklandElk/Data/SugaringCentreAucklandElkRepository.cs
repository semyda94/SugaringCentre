﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Data
{
    public class SugaringCentreAucklandElkRepository : ISugaringCentreAucklandElkRepository
    {
        private readonly SugaringCentreAucklandElkContext _DbContext;

        public SugaringCentreAucklandElkRepository(SugaringCentreAucklandElkContext ElkContext) =>
            _DbContext = ElkContext;

        #region Category

        public async Task<IEnumerable<Category>> GetListOfCategories()
        {
            return await _DbContext.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Category>> SearchCategoryByTitle(string searchedTitle)
        {
            return await (searchedTitle == null
                ? _DbContext.Categories.ToListAsync()
                : _DbContext.Categories.Where(x => x.Name.Contains(searchedTitle))
                    .ToListAsync());
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
        #endregion

        #region Product

        public async Task<List<Product>> GetListOfProducts()
        {
            return await _DbContext.Products.ToListAsync();
        }

        public async Task<List<Product>> GetProductsForCategoryAndSort(int? categoryId = -1, int? sorting = 1)
        {
            var notSorted = categoryId == -1
                ? await _DbContext.Products.Include(x => x.ProductImage).ToListAsync()
                : await _DbContext.ProductCategory.Include(x => x.ProductNavigation)
                    .ThenInclude(x => x.ProductImage)
                    .Where(i => i.CategoryId == categoryId)
                    .Select(x => x.ProductNavigation)
                    .Include(x => x.ProductImage)
                    .ToListAsync();

            switch (sorting)
            {
                case 3: return notSorted.OrderByDescending(x => x.Price).ToList();
                case 4: return notSorted.OrderBy(x => x.Price).ToList();
                default: return  notSorted;
            }
        }
        
        public async Task CreateProduct(Product product)
        {
            _DbContext.Products.Add(product);

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
        
        public async Task UpdateProduct(Product product)
        {
            _DbContext.Products.Update(product);
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
        
        public async Task<Product> GetProductById(int? productId)
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
        #endregion
        
        #region Staff

        public async Task<IEnumerable<Staff>> GetStaffList()
        {
            return await _DbContext.Staff.ToListAsync();
        }
        
        public async Task<List<Staff>> GetStaff(string searchName)
        {
            return await (searchName == null
                ? _DbContext.Staff.ToListAsync()
                : _DbContext.Staff.Where(x => (x.FirstName + ' ' + x.LastName).Contains(searchName))
                    .ToListAsync());
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
        
        public async Task UpdateStaff(Staff staff)
        {
            _DbContext.Staff.Update(staff);
            await _DbContext.SaveChangesAsync();
        }
        
        public async Task CreateStaff(Staff staff)
        {
            _DbContext.Staff.Add(staff);

            if (staff.ImagesToUpload.Any())
            {
                var imageToSafe = new List<StaffImage>();
                foreach (var image in staff.ImagesToUpload)
                {
                    if (image.Length > 0)
                    {
                       
                        using (var stream = new MemoryStream())
                        {
                            await image.CopyToAsync(stream);
                            imageToSafe.Add(new StaffImage
                            {
                                StaffId = staff.StaffId,
                                Image = stream.ToArray()
                            });
                        }
                    }
                }
                
                _DbContext.StaffImage.AddRange(imageToSafe);
            }

            await _DbContext.SaveChangesAsync();
        }

        #endregion

        #region ServicesCategory

        public async Task<string> GetServiceCategoryTitleById(int serviceCategoryId)
        {
            return (await _DbContext.ServiceCategory.SingleAsync(s => s.ServiceCategoryId == serviceCategoryId)).Title;
        }

        public async Task<IEnumerable<ServiceCategory>> GetServiceCategories()
        {
            return await _DbContext.ServiceCategory.ToListAsync();
        }

        public async Task DeleteServiceCategory(int serviceCategoryId)
        {
            var serviceCategory = _DbContext.ServiceCategory.Single(x => x.ServiceCategoryId == serviceCategoryId);
            _DbContext.ServiceCategory.Remove(serviceCategory);
            
            await _DbContext.SaveChangesAsync();
        }

        public async Task<ServiceCategory> GetServiceCategoryById(int serviceCategoryId)
        {
            return await _DbContext.ServiceCategory
                .SingleOrDefaultAsync(x => x.ServiceCategoryId == serviceCategoryId);
        }
        
        public async Task UpdateServiceCategory(ServiceCategory serviceCategory)
        {
            _DbContext.ServiceCategory.Update(serviceCategory);
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateServiceCategory(ServiceCategory serviceCategory)
        {
            _DbContext.ServiceCategory.Add(serviceCategory);

            await _DbContext.SaveChangesAsync();
        }

        #endregion

        #region Service

        public async Task<IEnumerable<Service>> GetServices()
        {
            return await _DbContext.Services.ToListAsync();
        }
        
        public async Task<List<Service>> GetServiceBySearchTitle(string searchTitle)
        {
            return await (searchTitle == null
                ? _DbContext.Services.ToListAsync()
                : _DbContext.Services.Where(x => x.Title.Contains(searchTitle))
                    .ToListAsync());
        }

        public async Task DeleteService(int serviceId)
        {
            var service = _DbContext.Services.Single(x => x.ServiceId == serviceId);
            _DbContext.Services.Remove(service);
            
            await _DbContext.SaveChangesAsync();
        }

        public async Task<Service> GetServiceById(int serviceId)
        {
            return await _DbContext.Services.SingleOrDefaultAsync(x => x.ServiceId == serviceId);
        }
        
        public async Task<IEnumerable<Service>> GetServiceForCategory(int serviceCategoryId)
        {
            return await _DbContext.Services.Where(x => x.ServiceCategoryId == serviceCategoryId).ToListAsync();
        }

        public async Task UpdateService(Service service)
        {
            _DbContext.Services.Update(service);
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateService(Service service)
        {
            _DbContext.Services.Add(service);
            
            if (service.SelectedStaff != null)
            {
                var splitedStaff = service.SelectedStaff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var serviceStaff = new List<ServiceStaff>();
                
                foreach (var staff in splitedStaff)
                {
                    serviceStaff.Add(new ServiceStaff()
                    {
                        StaffId = Int32.Parse(staff),
                        ServiceId = service.ServiceId
                    });
                }

                _DbContext.ServiceStaff.AddRange(serviceStaff);
            }
            
            if (service.ImagesToUpload.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await service.ImagesToUpload.CopyToAsync(stream);
                    service.Image = stream.ToArray();
                }
            }

            await _DbContext.SaveChangesAsync();
        }
        
        #endregion
        
        public async Task SubscribeForNews(string email)
        {
            _DbContext.Subscription.Add(new Subscription {Email = email});
            await _DbContext.SaveChangesAsync();
        }

        #region Booking

        public async Task CreateBooking(Booking booking)
        {
            booking.ServiceNavigation = null;
            booking.StaffNavigation = null;
            _DbContext.Bookings.Add(booking);
            await _DbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsForDate(int staffId, string dateToCheck)
        {
            var date = DateTime.ParseExact(dateToCheck, "d MMMM, yyyy", CultureInfo.InvariantCulture);
            return await _DbContext.Bookings.Where(x => x.StaffId == staffId && x.Date.Date == date.Date)
                .Include(x => x.ServiceNavigation)
                .ToListAsync();
        }

        #endregion
    }
}
