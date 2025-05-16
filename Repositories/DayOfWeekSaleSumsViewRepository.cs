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
    public class DayOfWeekSaleSumsViewRepository : IDayOfWeekSaleSumsViewRepository
    {
        private readonly StoreContext _context;
        public DayOfWeekSaleSumsViewRepository(StoreContext context )
        {
            _context = context;
        }

        public async Task<PagedModel<DayOfWeekSaleSumsView>> Get(int pageIndex, int pageSize)
        {
            try
            {
                var items = await _context.DayOfWeekSaleSumsView.ToListAsync();

                var totalItems = items.Count;

                items = items.Skip(pageIndex * pageSize)
                             .Take(pageSize)
                             .OrderBy(x => x.DayOfWeek).ToList();

                var result = new PagedModel<DayOfWeekSaleSumsView>()
                {
                    TotalItems = totalItems,
                    Items = items,
                };

                return result;
            }
            catch ( Exception ex)
            {
                throw;
            }
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
        //public async Task UpdayOfWeek(Book book)
        //{
        //    _context.Entry(book).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //}


    }
}
