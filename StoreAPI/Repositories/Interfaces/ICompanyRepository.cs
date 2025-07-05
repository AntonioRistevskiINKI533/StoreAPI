using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> GetByNameAddressOrPhone(string name, string address, string phone, int? companyId = null);
        Task<Company> Add(Company company);
        Task<Company> Update(Company company);
        Task<PagedModel<Company>> GetAllPaged(int pageIndex, int pageSize);
        Task<Company> GetById(int id);
        Task Remove(Company company);
    }
}
