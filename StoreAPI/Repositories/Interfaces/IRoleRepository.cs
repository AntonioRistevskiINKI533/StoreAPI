using StoreAPI.Models;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAll();
        Task<Role> GetById(int id);
    }
}
