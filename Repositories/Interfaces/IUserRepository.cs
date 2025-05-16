using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsername(string username);
    }
}
