using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByName(string name);
        Task<Product> GetByRegistrationNumber(string registrationNumber);
        Task<Product> GetByCompanyId(int companyId);
        Task<Product> Add(Product product);
        Task<Product> Update(Product product);
        Task<PagedModel<ProductData>> GetAllPaged(int pageIndex, int pageSize, int? companyId);
        Task<Product> GetById(int id);
        Task Remove(Product product);
    }
}
