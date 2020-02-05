using System;
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
        
        public async Task<List<Staff>> GetStaff(string searchName)
        {
            return await (searchName == null ? _DbContext.Staff.ToListAsync() : _DbContext.Staff.Where(x => (x.FirstName + ' ' + x.LastName).Contains(searchName)).ToListAsync());
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

        #region Services

        public async Task<IEnumerable<Service>> GetServices()
        {
            return await _DbContext.Service.ToListAsync();
        }

        public async Task DeleteService(int serviceId)
        {
            var service = _DbContext.Service.Single(x => x.ServiceId == serviceId);
            _DbContext.Service.Remove(service);
            
            await _DbContext.SaveChangesAsync();
        }

        public async Task<Service> GetService(int serviceId)
        {
            return await _DbContext.Service.SingleOrDefaultAsync(x => x.ServiceId == serviceId);
        }
        
        public async Task UpdateService(Service service)
        {
            _DbContext.Service.Update(service);
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateService(Service service)
        {
            _DbContext.Service.Add(service);

            await _DbContext.SaveChangesAsync();
        }

        #endregion

        #region ServiceTypes

        public async Task<IEnumerable<ServiceType>> GetServiceTypes()
        {
            return await _DbContext.ServiceType.ToListAsync();
        }
        
        public async Task<IEnumerable<ServiceType>> GetServiceTypesForService(int serviceId)
        {
            return await _DbContext.ServiceType.Where(x => x.ServiceId == serviceId).ToListAsync();
        }
        
        public async Task<List<Service>> GetServiceTypes(string searchName)
        {
            return await (searchName == null ? _DbContext.Service.ToListAsync() : _DbContext.Service.Where(x => x.Name.Contains(searchName)).ToListAsync());
        }

        public async Task DeleteServiceType(int serviceTypeId)
        {
            var serviceType = _DbContext.ServiceType.Single(x => x.ServiceTypeId == serviceTypeId);
            _DbContext.ServiceType.Remove(serviceType);
            
            await _DbContext.SaveChangesAsync();
        }

        public async Task<ServiceType> GetServiceType(int serviceTypeId)
        {
            return await _DbContext.ServiceType.SingleOrDefaultAsync(x => x.ServiceTypeId == serviceTypeId);
        }

        public async Task UpdateServiceType(ServiceType serviceType)
        {
            _DbContext.ServiceType.Update(serviceType);
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateServiceType(ServiceType serviceType)
        {
            _DbContext.ServiceType.Add(serviceType);
            
            if (serviceType.SelectedStaff != null)
            {
                var splitedStaff = serviceType.SelectedStaff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var serviceTypeStaff = new List<ServiceTypeStaff>();
                
                foreach (var staff in splitedStaff)
                {
                    serviceTypeStaff.Add(new ServiceTypeStaff
                    {
                        Staff = Int32.Parse(staff),
                        ServiceType = serviceType.ServiceTypeId
                    });
                }

                _DbContext.ServiceTypeStaff.AddRange(serviceTypeStaff);
            }
            
            if (serviceType.ImagesToUpload.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await serviceType.ImagesToUpload.CopyToAsync(stream);
                    serviceType.Tmbnail = stream.ToArray();
                }
            }

            await _DbContext.SaveChangesAsync();
        }
        
        #endregion
    }
}
