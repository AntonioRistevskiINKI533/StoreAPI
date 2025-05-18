using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> GetByAddressOrPhone(string address, string phone);
        Task<Company> Add(Company company);
        Task<Company> Update(Company company);
        Task<PagedModel<Company>> GetAllPaged(int pageIndex, int pageSize);
        Task<Company> GetById(int id);
        Task Remove(Company company);
    }
}
