using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;
using BCrypt.Net;

namespace StoreAPI.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleData>> GetAllRoles()
        {
            var roles = await _roleRepository.GetAll();

            var result = roles.Select(role => new RoleData {
                Id = role.Id,
                Name = role.Name,
            }).ToList();

            return result;
        }
    }
}
