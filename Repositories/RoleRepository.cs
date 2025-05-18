using StoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Repositories.Interfaces;
using StoreAPI.Models.Contexts;

namespace StoreAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly StoreContext _context;
        public RoleRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAll()
        {
            var items = await _context.Role.ToListAsync();

            return items;
        }
    }
}
