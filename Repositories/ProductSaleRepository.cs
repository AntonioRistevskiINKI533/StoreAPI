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

        public async Task<ProductSale> GetByProductId(int productId)
        {
            return await _context.ProductSale.Where(x => x.ProductId == productId).FirstOrDefaultAsync();
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

        public async Task<PagedModel<ProductSaleData>> GetAllPaged(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo, int? productId)
        {
            var query = from sale in _context.ProductSale
                         join product in _context.Product
                             on sale.ProductId equals product.Id
                         where (productId == null || sale.ProductId == productId) &&
                               (dateFrom == null || sale.Date >= dateFrom) &&
                               (dateTo == null || sale.Date <= dateTo)
                         select new ProductSaleData
                         {
                             Id = sale.Id,
                             ProductId = sale.ProductId,
                             SoldAmount = sale.SoldAmount,
                             PricePerUnit = sale.PricePerUnit,
                             Date = sale.Date,
                             ProductName = product.Name
                         };

            var totalItems = await query.CountAsync();

            var items = query.Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new PagedModel<ProductSaleData>()
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
