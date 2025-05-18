using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameOrEmail(string username, string email);
        Task<User> Add(User user);
        Task<User> Update(User user);
        Task<PagedModel<User>> GetAllPaged(int pageIndex, int pageSize);
        Task<User> GetById(int id);
        Task Remove(User user);
    }
}
