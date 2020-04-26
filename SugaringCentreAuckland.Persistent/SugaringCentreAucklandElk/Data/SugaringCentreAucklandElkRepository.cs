using System;
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
            return await _DbContext.Categories.Include(x => x.ProductCategory).ToListAsync();
        }

        public async Task<IEnumerable<Category>> SearchCategoryByTitle(string searchedTitle)
        {
            return await (searchedTitle == null
                ? _DbContext.Categories.ToListAsync()
                : _DbContext.Categories.Where(x => x.Name.Contains(searchedTitle))
                    .ToListAsync());
        }
        
        public async Task<IEnumerable<ServiceCategory>> SearchServiceCategoryByTitle(string searchedTitle)
        {
            return await (searchedTitle == null
                ? _DbContext.ServiceCategory.ToListAsync()
                : _DbContext.ServiceCategory.Where(x => x.Title.Contains(searchedTitle))
                    .ToListAsync());
        }
        
        public async Task DeleteCategory(int categoryId)
        {
            var productCategories = _DbContext.ProductCategory.Where(x => x.CategoryId == categoryId);
            var category = _DbContext.Categories.Single(c => c.CategoryId == categoryId);

            if (productCategories.Any())
                _DbContext.ProductCategory.RemoveRange(productCategories);
            
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
            product.ProductId = 0;
            _DbContext.Products.Add(product);

            await _DbContext.SaveChangesAsync();
            
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

            if (product.ImagesToUpload != null && product.ImagesToUpload.Any())
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
            var productToUpdate = _DbContext.Products.Single(x => x.ProductId == product.ProductId);

            productToUpdate.Title = product.Title;
            productToUpdate.Desc = product.Desc;
            productToUpdate.Price = productToUpdate.Price;
         
            _DbContext.Products.Update(productToUpdate);
            
            await _DbContext.SaveChangesAsync();
        }
        
        public async Task DeleteProduct(int? productId)
        {
            var productCategories = await _DbContext.ProductCategory.Where(x => x.ProductId == productId).ToListAsync();
            var productImages = await _DbContext.ProductImage.Where(x => x.ProductId == productId).ToListAsync();
            var product = _DbContext.Products.Single(p => p.ProductId == productId);
            
            if (productCategories.Any())
                _DbContext.ProductCategory.RemoveRange(productCategories);
            
            if (productImages.Any())
                _DbContext.ProductImage.RemoveRange(productImages);
            
            _DbContext.Products.Remove(product);

            await _DbContext.SaveChangesAsync();
        }
        
        public async Task<Product> GetProductById(int? productId)
        {
            var product = await _DbContext.Products.Include(i => i.ProductImage)
                .Include(x => x.ProductCategory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            // if (product.ProductCategory.Any())
            // {
            //     foreach (var category in product.ProductCategory)
            //     {
            //         product.CategorySelected.
            //     }
            // }

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
        
        public async Task<Staff> GetStaffWithLeaves(int staffId)
        {
            return await _DbContext.Staff.Where(x => x.StaffId == staffId)
                .Include(x => x.Leaves)
                .Include(x => x.StaffImage)
                .SingleOrDefaultAsync();
        }

        public async Task DeleteStaff(int staffId)
        {
            var staff = _DbContext.Staff.SingleOrDefault(x => x.StaffId == staffId);
            var staffImageToDelete = _DbContext.StaffImage.Where(x => x.StaffId == staffId);

            if (staff != null)
            {
                _DbContext.Staff.Remove(staff);
                _DbContext.StaffImage.RemoveRange(staffImageToDelete);
                await _DbContext.SaveChangesAsync();
            }
        }
        
        public async Task UpdateStaff(Staff staff)
        {
            var staffToUpdate = _DbContext.Staff.Single(x => x.StaffId == staff.StaffId);

            staffToUpdate.FirstName = staff.FirstName;
            staffToUpdate.LastName = staff.LastName;
            staffToUpdate.Title = staff.Title;
            staffToUpdate.Dob = staff.Dob;
            staffToUpdate.WorkingDaysOfWeek = staff.WorkingDaysOfWeek; 
            
            if (staff.ImagesToUpload != null && staff.ImagesToUpload.Any())
            {
                var staffImagesToDelete = _DbContext.StaffImage.Where(x => x.StaffId == staff.StaffId).ToList();
                _DbContext.StaffImage.RemoveRange(staffImagesToDelete);
                
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
            
            _DbContext.Staff.Update(staffToUpdate);
            await _DbContext.SaveChangesAsync();
        }
        
        public async Task CreateStaff(Staff staff)
        {
            _DbContext.Staff.Add(staff);

            if (staff.ImagesToUpload != null && staff.ImagesToUpload.Any())
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

        public async Task<IEnumerable<Staff>> GetStaffForService(int serviceId)
        {
            return await _DbContext.ServiceStaff.Where(ss => ss.ServiceId == serviceId)
                .Include(ss => ss.StaffNavigation)
                .Select(ss => ss.StaffNavigation)
                .ToListAsync(); 
        }

        public async Task<IEnumerable<Leave>> GetLeavesForStaff(int staffId)
        {
            return await _DbContext.Leave.Where(x => x.StaffId == staffId).ToListAsync();;
        }
        #endregion

        #region ServicesCategory

        public async Task<string> GetServiceCategoryTitleById(int serviceCategoryId)
        {
            return (await _DbContext.ServiceCategory.SingleAsync(s => s.ServiceCategoryId == serviceCategoryId)).Title;
        }

        public async Task<IEnumerable<ServiceCategory>> GetServiceCategoriesWithRelatedServices()
        {
            return await _DbContext.ServiceCategory.Include(x => x.Services).ToListAsync();
        }

        public async Task DeleteServiceCategory(int serviceCategoryId)
        {
            var serviceCategory = _DbContext.ServiceCategory.Single(x => x.ServiceCategoryId == serviceCategoryId);

            var services = _DbContext.Services.Where(x => x.ServiceCategoryId == serviceCategoryId);
            
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
            var serviceCategoryToUpdate = _DbContext.ServiceCategory.Single(x => x.ServiceCategoryId == serviceCategory.ServiceCategoryId);

            serviceCategoryToUpdate.Title = serviceCategory.Title;
            serviceCategoryToUpdate.Description = serviceCategory.Description;
            
            _DbContext.ServiceCategory.Update(serviceCategoryToUpdate);
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateServiceCategory(ServiceCategory serviceCategory)
        {
            serviceCategory.ServiceCategoryId = 0;
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

            var serviceStaff = _DbContext.ServiceStaff.Where(x => x.ServiceId == serviceId);

            _DbContext.ServiceStaff.RemoveRange(serviceStaff);
            _DbContext.Services.Remove(service);
            
            await _DbContext.SaveChangesAsync();
        }

        public async Task<Service> GetServiceById(int serviceId)
        {
            return await _DbContext.Services.Include(x => x.ServiceCategoryNavigation)
                .SingleOrDefaultAsync(x => x.ServiceId == serviceId);
        }
        
        public async Task<IEnumerable<Service>> GetServiceForCategory(int serviceCategoryId)
        {
            return await _DbContext.Services.Where(x => x.ServiceCategoryId == serviceCategoryId).ToListAsync();
        }

        public async Task UpdateService(Service service)
        {
            var serviceToUpdate = _DbContext.Services.Include(x => x.ServiceStaff)
                .Single(x => x.ServiceId == service.ServiceId);

            serviceToUpdate.Title = service.Title;
            serviceToUpdate.Desc = service.Desc;
            serviceToUpdate.Price = service.Price;
            serviceToUpdate.Duration = service.Duration;
            serviceToUpdate.ServiceCategoryId = service.ServiceCategoryId;
            
            if (service.ImagesToUpload != null && service.ImagesToUpload.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await service.ImagesToUpload.CopyToAsync(stream);
                    serviceToUpdate.Image = stream.ToArray();
                }
            }
            
            if (service.SelectedStaff != null)
            {
                var existedStaff = serviceToUpdate.ServiceStaff.Select(x => x.StaffId);
                var splitedStaff = service.SelectedStaff.Split(',',
                        StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Int32.Parse(x));

                var serviceStaffIdsToAdd = splitedStaff.Where(x => !existedStaff.Contains(x));
                var serviceStaffIdsToDelete = existedStaff.Where(x => !splitedStaff.Contains(x));

                if (serviceStaffIdsToAdd.Any())
                {
                    var serviceStaffToAdd = new List<ServiceStaff>();
                    
                    foreach (var staff in serviceStaffIdsToAdd)
                    {
                        serviceStaffToAdd.Add(new ServiceStaff()
                        {
                            StaffId = staff,
                            ServiceId = service.ServiceId
                        });
                    }

                    _DbContext.ServiceStaff.AddRange(serviceStaffToAdd);
                }
                
                if (serviceStaffIdsToDelete.Any())
                {
                    var serviceStaffToDelete =
                        serviceToUpdate.ServiceStaff.Where(x => serviceStaffIdsToDelete.Contains(x.StaffId));
                    
                    _DbContext.ServiceStaff.RemoveRange(serviceStaffToDelete);
                }
                
            }
            
            _DbContext.Services.Update(serviceToUpdate);
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateService(Service service)
        {
            _DbContext.Services.Add(service);

            await _DbContext.SaveChangesAsync();
            
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

        public async Task<IEnumerable<Booking>> GetBookingsForStaff(int staffId)
        {
            return await _DbContext.Bookings.Where(x => x.StaffId == staffId)
                .Include(x => x.ServiceNavigation)
                .ToListAsync();
        }

        public Booking GetBooking(int bookingId)
        {
            return _DbContext.Bookings.Where(x => x.BookingId == bookingId)
                .Include(x => x.ServiceNavigation)
                .Include(x => x.StaffNavigation)
                .SingleOrDefault();
        }
        
        #endregion

        #region Leave

        public async Task DeleteLeave(int leaveId)
        {
            var leaveToDelete = _DbContext.Leave.Where(x => x.LeaveId == leaveId).SingleOrDefault();
            _DbContext.Leave.Remove(leaveToDelete);
            await _DbContext.SaveChangesAsync();
        }

        public async Task CreateLeave(int staffId, DateTime date, string reason)
        {
            _DbContext.Leave.Add(new Leave
            {
                StaffId = staffId,
                Date = date
            });

            await _DbContext.SaveChangesAsync();
        }

        #endregion
    }
}
