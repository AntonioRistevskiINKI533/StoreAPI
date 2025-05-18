using StoreAPI.Models;
using StoreAPI.Models.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Repositories.Interfaces;
using StoreAPI.Models.Contexts;

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

        public async Task<PagedModel<User>> GetAllPaged(int pageIndex, int pageSize)
        {
            var items = await _context.User.ToListAsync();

            var totalItems = items.Count;

            items = items.Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new PagedModel<User>()
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

        //public  async Task<Book> Create(Book book)
        //{
        //    _context.Books.Add(book);
        //    await _context.SaveChangesAsync();
        //    return book;
        //}
        //
        //public  async Task Delete(int id)
        //{
        //    var bookToDelete = await _context.Books.FindAsync(id);
        //    _context.Books.Remove(bookToDelete);
        //    await _context.SaveChangesAsync();
        //}
        //
        //
        //
        //public async Task<IEnumerable<Book>> Get()
        //{
        //    return await _context.Books.ToListAsync();
        //
        //}
        //
        //public async Task<Book> Get(int id)
        //{
        //    return await _context.Books.FindAsync(id);
        //
        //}
        //
        //public async Task Update(Book book)
        //{
        //    _context.Entry(book).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //}


    }
}
