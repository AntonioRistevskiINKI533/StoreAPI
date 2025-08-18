using StoreAPI.Models;
using StoreAPI.Models.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Repositories.Interfaces;
using StoreAPI.Models.Contexts;
using System.ComponentModel.Design;

namespace StoreAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreContext _context;
        public UserRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<User> GetByUsernameOrEmail(string username, string email, int? userId = null)
        {
            return await _context.User.Where(x => (x.Username == username || x.Email == email) && x.Id != userId).FirstOrDefaultAsync();
        }

        public async Task<User> Add(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<PagedModel<UserData>> GetAllPaged(int pageIndex, int pageSize, string? fullName, int? roleId)
        {
            var query = from user in _context.User
                        join role in _context.Role
                            on user.RoleId equals role.Id
            where (roleId == null || user.RoleId == roleId) &&
                        (fullName == null || 
                        (user.Name.Trim() + ' ' + user.Surname.Trim()).ToLower().Contains(fullName.Trim().ToLower()) || 
                        (user.Username.Trim().ToLower().Contains(fullName.Trim().ToLower())))
                        select new UserData
                        {
                            Id = user.Id,
                            Username = user.Username,
                            Email = user.Email,
                            Name = user.Name,
                            Surname = user.Surname,
                            RoleId = user.RoleId,
                            RoleName = role.Name
                        };

            var totalItems = await query.CountAsync();

            var items = query.Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new PagedModel<UserData>()
            {
                TotalItems = totalItems,
                Items = items,
            };

            return result;
        }

        public async Task<User> GetById(int id)
        {
            return await _context.User.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Remove(User user)
        {
           _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }

    }
}
