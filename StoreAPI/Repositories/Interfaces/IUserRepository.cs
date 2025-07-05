using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameOrEmail(string username, string email, int? userId = null);
        Task<User> Add(User user);
        Task<User> Update(User user);
        Task<PagedModel<UserData>> GetAllPaged(int pageIndex, int pageSize, string? fullName, int? roleId);
        Task<User> GetById(int id);
        Task Remove(User user);
    }
}
