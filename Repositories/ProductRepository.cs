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
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;
        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByName(string name)
        {
            return await _context.Product.Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<Product> GetByRegistrationNumber(string registrationNumber)
        {
            return await _context.Product.Where(x => x.RegistrationNumber == registrationNumber).FirstOrDefaultAsync();
        }

        public async Task<Product> GetByCompanyId(int companyId)
        {
            return await _context.Product.Where(x => x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<Product> Add(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> Update(Product product)
        {
            _context.Product.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<PagedModel<Product>> GetAllPaged(int pageIndex, int pageSize)
        {
            var items = await _context.Product.ToListAsync();

            var totalItems = items.Count;

            items = items.Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new PagedModel<Product>()
            {
                TotalItems = totalItems,
                Items = items,
            };

            return result;
        }

        public async Task<Product> GetById(int id)
        {
            return await _context.Product.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Remove(Product product)
        {
           _context.Product.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
