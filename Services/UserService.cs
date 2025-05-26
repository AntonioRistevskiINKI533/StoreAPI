using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;
using BCrypt.Net;
using StoreAPI.Enums;

namespace StoreAPI.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;

        public UserService(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameOrEmail(request.Username, null);// login only possible with username

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var a = ((RoleEnum)user.RoleId).ToString();

            var token = _tokenService.GenerateToken(user.Id, a);

            return new LoginResponse
            {
                Token = token
            };
        }

        public async Task<UserData> GetUserProfile(int userId)
        {
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            return new UserData
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                RoleId = user.RoleId,
            };
        }

        public async Task UpdateUserProfile(UpdateUserProfileRequest request, int userId)
        {
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            var existingUser = await _userRepository.GetByUsernameOrEmail(request.Username, request.Email, user.Id);

            if (existingUser != null)
            {
                if (existingUser.Username == request.Username)
                {
                    throw new Exception("User with same username already exists");
                }

                if (existingUser.Email == request.Email)
                {
                    throw new Exception("User with same email already exists");
                }
            }

            user.Username = request.Username;
            user.Email = request.Email;
            user.Name = request.Name;
            user.Surname = request.Surname;

            await _userRepository.Update(user);

            return;
        }

        public async Task AddUser(AddUserRequest request)
        {
            var user = await _userRepository.GetByUsernameOrEmail(request.Username, request.Email);

            if (user != null)
            {
                if (user.Username == request.Username)
                {
                    throw new Exception("User with same username already exists");
                }

                if (user.Email == request.Email)
                {
                    throw new Exception("User with same email already exists");
                }
            }

            user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = request.Name,
                Surname = request.Surname,
                RoleId = request.RoleId
            };

            await _userRepository.Add(user);

            return;
        }

        public async Task UpdateUser(UpdateUserRequest request)
        {
            var user = await _userRepository.GetById(request.Id);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            var existingUser = await _userRepository.GetByUsernameOrEmail(request.Username, request.Email, user.Id);

            if (existingUser != null)
            {
                if (existingUser.Username == request.Username)
                {
                    throw new Exception("User with same username already exists");
                }

                if (existingUser.Email == request.Email)
                {
                    throw new Exception("User with same email already exists");
                }
            }

            user.Username = request.Username;
            user.Email = request.Email;
            user.Name = request.Name;
            user.Surname = request.Surname;

            await _userRepository.Update(user);

            return;
        }

        public async Task<UserData> GetUser(int userId)
        {
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            return new UserData
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                RoleId = user.RoleId,
            };
        }

        public async Task<PagedModel<UserData>> GetAllUsersPaged(int pageIndex, int pageSize, string? fullName, int? roleId)
        {
            var users = await _userRepository.GetAllPaged(pageIndex, pageSize, fullName, roleId);

            var userData = users.Items.Select(x => new UserData
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                Name = x.Name,
                Surname = x.Surname,
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToList();

            var result = new PagedModel<UserData>()
            {
                TotalItems = users.TotalItems,
                Items = userData
            };

            return result;
        }

        public async Task RemoveUser(int userId)
        {
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            await _userRepository.Remove(user);

            return;
        }
    }
}
