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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly StoreContext _context;
        public CompanyRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Company> GetByNameAddressOrPhone(string name, string address, string phone, int? companyId = null)
        {
            return await _context.Company.Where(x => (x.Name == name || x.Address == address || x.Phone == phone) && x.Id != companyId).FirstOrDefaultAsync();
        }

        public async Task<Company> Add(Company company)
        {
            _context.Company.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company> Update(Company company)
        {
            _context.Company.Update(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<PagedModel<Company>> GetAllPaged(int pageIndex, int pageSize)
        {
            var items = await _context.Company.ToListAsync();

            var totalItems = items.Count;

            items = items.Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

            var result = new PagedModel<Company>()
            {
                TotalItems = totalItems,
                Items = items,
            };

            return result;
        }

        public async Task<Company> GetById(int id)
        {
            return await _context.Company.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Remove(Company company)
        {
           _context.Company.Remove(company);
            await _context.SaveChangesAsync();
        }
    }
}
