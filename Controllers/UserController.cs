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
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;

        public UserController(ILogger<UserController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                var result = await _userService.Login(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ActionResult<UserData>), 200)]
        public async Task<ActionResult<UserData>> GetUserProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // ASP.NET Core maps sub (JWT standard) to ClaimTypes.NameIdentifier by default.

                if (userIdClaim == null)
                {
                    return Unauthorized("User Id claim not found");
                }

                int userId = int.Parse(userIdClaim.Value);

                var result = await _userService.GetUserProfile(userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> UpdateUserProfile(UpdateUserProfileRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // ASP.NET Core maps sub (JWT standard) to ClaimTypes.NameIdentifier by default.

                if (userIdClaim == null)
                {
                    return Unauthorized("User Id claim not found");
                }

                int userId = int.Parse(userIdClaim.Value);

                await _userService.UpdateUserProfile(request, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> AddUser(AddUserRequest request)
        {
            try
            {
                await _userService.AddUser(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> UpdateUser(UpdateUserRequest request)
        {
            try
            {
                await _userService.UpdateUser(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ActionResult<UserData>), 200)]
        public async Task<ActionResult<UserData>> GetUser(int userId)
        {
            try
            {
                var user = await _userService.GetUser(userId);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<UserData>), 200)]
        public async Task<ActionResult<PagedModel<UserData>>> GetAllUsersPaged(int pageIndex, int pageSize, string? fullName, int? roleId)
        {
            try
            {
                var users = await _userService.GetAllUsersPaged(pageIndex, pageSize, fullName, roleId);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> RemoveUser(int userId)
        {
            try
            {
                await _userService.RemoveUser(userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}