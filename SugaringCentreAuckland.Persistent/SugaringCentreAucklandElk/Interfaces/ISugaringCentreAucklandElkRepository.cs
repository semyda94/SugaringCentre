
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
        Task<List<Product>> GetproductsForCategory(int? categoryId = -1, int? sorting = 1);
        Task DeleteCategory(int categoryId);
        Task CreatCategory(string categoryName);
        Task CreateProduct(Product product, string projectWebRootPath);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int? productId);
        Task<Product> GetShopItem(int? productId);
        Task SubscribeForNews(string email);

        #region Staff

        Task<IEnumerable<Staff>> GetStaffList();
        Task<List<Staff>> GetStaff(string searchName);
        Task<Staff> GetStaff(int staffId);
        Task DeleteStaff(int staffId);
        Task UpdateStaff(Staff staff);
        Task CreateStaff(Staff staff);

        #endregion

        #region Service

        Task<IEnumerable<Service>> GetServices();
        Task DeleteService(int serviceId);
        Task<Service> GetService(int serviceId);
        Task CreateService(Service service);
        Task UpdateService(Service service);
        #endregion

        #region ServiceType

        Task<IEnumerable<ServiceType>> GetServiceTypes();
        Task<List<Service>> GetServiceTypes(string searchName);

        Task DeleteServiceType(int serviceTypeId);
        Task<ServiceType> GetServiceType(int serviceTypeId);
        Task<IEnumerable<ServiceType>> GetServiceTypesForService(int serviceId);
        Task CreateServiceType(ServiceType serviceType);
        Task UpdateServiceType(ServiceType serviceType);
        #endregion
        
    }
}
