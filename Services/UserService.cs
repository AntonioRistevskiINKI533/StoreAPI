using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;
using BCrypt.Net;

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
            var user = await _userRepository.GetByUsernameOrEmail(request.Username, request.Username);//TODO fix

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = _tokenService.GenerateToken(user.Id);

            return new LoginResponse
            {
                Token = token
            };
        }

        public async Task<GetUserProfileResponse> GetUserProfile(int userId)
        {
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            return new GetUserProfileResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            };
        }

        public async Task UpdateUserProfile(UpdateUserProfileRequest request, int userId)
        {
            if (request.Email.Contains('@') == false)
            {
                throw new Exception("Invalid email");
            }

            if (request.Username.Length > 20 || request.Username.Length < 5)
            {
                throw new Exception("Username must be between 5 to 20 characters long");
            }

            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            //add validation for unique

            user.Username = request.Username;
            user.Email = request.Email;

            await _userRepository.Update(user);

            return;
        }

        public async Task AddUser(AddUserRequest request)
        {
            if (request.Email.Contains('@') == false)
            {
                throw new Exception("Invalid email");
            }

            if (request.Username.Length > 20 || request.Username.Length < 5)
            {
                throw new Exception("Username must be between 5 to 20 characters long");
            }

            if (request.Password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters long");
            }

            var user = await _userRepository.GetByUsernameOrEmail(request.Username, request.Email);

            if (user != null)
            {
                throw new Exception("User with same email or username already exists");
            }

            user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.Add(user);

            return;
        }

        public async Task UpdateUser(UpdateUserRequest request)
        {
            if (request.Email.Contains('@') == false)
            {
                throw new Exception("Invalid email");
            }

            if (request.Username.Length > 20 || request.Username.Length < 5)
            {
                throw new Exception("Username must be between 5 to 20 characters long");
            }

            var user = await _userRepository.GetById(request.Id);

            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            //add validation for unique

            user.Username = request.Username;
            user.Email = request.Email;

            await _userRepository.Update(user);

            return;
        }

        public async Task<PagedModel<UserData>> GetAllUsersPaged(int pageIndex, int pageSize)
        {
            var users = await _userRepository.GetAllPaged(pageIndex, pageSize);

            var userData = users.Items.Select(x => new UserData
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email
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
