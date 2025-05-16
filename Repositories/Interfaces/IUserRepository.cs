using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameOrEmail(string usernameOrEmail);
        Task<User> AddUser(User user);
    }
}
