
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces
{
    public interface ISugaringCentreAucklandElkRepository
    {
        #region Category

        Task<IEnumerable<Category>> GetListOfCategories();
        Task<IEnumerable<Category>> SearchCategoryByTitle(string searchedTitle);
        Task DeleteCategory(int categoryId);
        Task CreatCategory(string categoryName);
        
        #endregion

        #region Product

        Task<List<Product>> GetListOfProducts();
        Task<List<Product>> GetProductsForCategoryAndSort(int? categoryId = -1, int? sorting = 1);
        Task CreateProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int? productId);
        Task<Product> GetProductById(int? productId);
        #endregion
        
        #region Staff

        Task<IEnumerable<Staff>> GetStaffList();
        Task<List<Staff>> GetStaff(string searchName);
        Task<Staff> GetStaff(int staffId);
        Task<Staff> GetStaffWithLeaves(int staffId);
        Task DeleteStaff(int staffId);
        Task UpdateStaff(Staff staff);
        Task CreateStaff(Staff staff);
        Task<IEnumerable<Staff>> GetStaffForService(int serviceId);
        Task<IEnumerable<Leave>> GetLeavesForStaff(int staffId);

        #endregion

        #region ServiceCategory

        Task<IEnumerable<ServiceCategory>> SearchServiceCategoryByTitle(string searchedTitle);
        Task<string> GetServiceCategoryTitleById(int serviceCategoryId);
        Task<IEnumerable<ServiceCategory>> GetServiceCategoriesWithRelatedServices();
        Task DeleteServiceCategory(int serviceCategoryId);
        Task<ServiceCategory> GetServiceCategoryById(int serviceCategoryId);
        Task CreateServiceCategory(ServiceCategory serviceCategory);
        Task UpdateServiceCategory(ServiceCategory serviceCategory);
        #endregion

        #region Service

        Task<IEnumerable<Service>> GetServices();
        Task<List<Service>> GetServiceBySearchTitle(string searchTitle);
        Task DeleteService(int serviceId);
        Task<Service> GetServiceById(int serviceId);
        Task<IEnumerable<Service>> GetServiceForCategory(int serviceCategoryId);
        Task UpdateService(Service service);
        Task CreateService(Service service);
        #endregion
        
        Task SubscribeForNews(string email);

        #region Booking

        Task CreateBooking(Booking booking);
        Task DeleteBooking(int bookingId);
        Task UpdateBooking(Booking booking);
        Task<IEnumerable<Booking>> GetBookingsForDate(int staffId, string dateToCheck);
        Task<IEnumerable<Booking>> GetBookingsForStaff(int staffId);
        Booking GetBooking(int bookingId);

        int GetBookingsNumber();

        #endregion

        #region Leave

        Task DeleteLeave(int leaveId);
        Task CreateLeave(int staffId, DateTime date, string reason);

        #endregion

        #region Login

        Staff ValidateLoginModel(string username, string encryptedPassword);
        #endregion


        #region Orders

        Task CreateOrder(Order order);

        #endregion
        
        // int GetOrdersNumber();
        // int GetOrdersValue();
        IEnumerable<Tuple<string,int>> GetTopBookingsPerMaster();
        IEnumerable<Tuple<string,int>> GetTopBookingsPerService();
    }
}
