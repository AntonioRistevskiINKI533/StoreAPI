using StoreAPI.Services;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {

        private readonly ILogger<RoleController> _logger;
        private readonly RoleService _roleService;

        public RoleController(ILogger<RoleController> logger, RoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ActionResult<List<RoleData>>), 200)]
        public async Task<ActionResult<List<RoleData>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRoles();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}