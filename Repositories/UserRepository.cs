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

        public async Task<User> GetByUsername(string username)
        {
            return await _context.User.Where(x => x.Username == username).FirstOrDefaultAsync();
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
