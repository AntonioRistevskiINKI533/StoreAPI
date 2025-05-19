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
    public class ProductSaleRepository : IProductSaleRepository
    {
        private readonly StoreContext _context;
        public ProductSaleRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<ProductSale> Add(ProductSale productSale)
        {
            _context.ProductSale.Add(productSale);
            await _context.SaveChangesAsync();
            return productSale;
        }

        public async Task<ProductSale> Update(ProductSale productSale)
        {
            _context.ProductSale.Update(productSale);
            await _context.SaveChangesAsync();
            return productSale;
        }

        public async Task<PagedModel<ProductSale>> GetAllPaged(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo, int? productId)
        {
            var items = await _context.ProductSale.Where(x => 
                (x.ProductId == productId || productId == null) &&
                (x.Date >= dateFrom || dateFrom == null) &&
                (x.Date <= dateTo || dateTo == null)).ToListAsync();

            var totalItems = items.Count;

            items = items.Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new PagedModel<ProductSale>()
            {
                TotalItems = totalItems,
                Items = items,
            };

            return result;
        }

        public async Task<ProductSale> GetById(int id)
        {
            return await _context.ProductSale.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Remove(ProductSale productSale)
        {
           _context.ProductSale.Remove(productSale);
            await _context.SaveChangesAsync();
        }
    }
}
