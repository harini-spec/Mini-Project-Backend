using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace BusBookingAppln.Services.Classes
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, UserDetail> _userDetailRepo;
        private readonly ITokenService _tokenService;

        public UserService(IRepository<int, User> userRepo, IRepository<int, UserDetail> userDetailRepo, ITokenService tokenService)
        {
            _userRepo = userRepo;
            _userDetailRepo = userDetailRepo;
            _tokenService = tokenService;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var users = await _userRepo.GetAll();
            var user = users.ToList().FirstOrDefault(x => x.Email == email);
            if(user == null)
            {
                throw new UnauthorizedUserException("Invalid username or password");
            }
            return user;
        }

        public async Task<LoginOutputDTO> LoginAdminAndCustomer(LoginInputDTO loginInputDTO)
        {
            try
            {
                User user = await GetUserByEmail(loginInputDTO.Email);
                UserDetail userDetail = await _userDetailRepo.GetById(user.Id);

                HMACSHA512 hMACSHA = new HMACSHA512(userDetail.PasswordHashKey);
                var encryptedPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginInputDTO.Password));
                bool isPasswordSame = ComparePassword(encryptedPass, userDetail.PasswordEncrypted);
                if (isPasswordSame)
                {
                    if (userDetail.Status == "Active")
                    {
                        LoginOutputDTO loginOutputDTO = MapUserToLoginReturnDTO(user);
                        return loginOutputDTO;
                    }
                    throw new UserNotActiveException("Your account is not activated");
                }
                throw new UnauthorizedUserException("Invalid username or password");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private LoginOutputDTO MapUserToLoginReturnDTO(User user)
        {
            LoginOutputDTO loginOutputDTO = new LoginOutputDTO();
            loginOutputDTO.UserID = user.Id;
            loginOutputDTO.UserName = user.Name;
            loginOutputDTO.Token = _tokenService.GenerateToken(user);
            loginOutputDTO.Role = user.Role;
            return loginOutputDTO;
        }

        private bool ComparePassword(byte[] encryptedPass, byte[] passwordEncrypted)
        {
            for (int i = 0; i < encryptedPass.Length; i++)
            {
                if (encryptedPass[i] != passwordEncrypted[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<RegisterOutputDTO> RegisterAdminAndCustomer(RegisterInputDTO registerInputDTO, string Role)
        {
            User user = null;
            User InsertedUser = null;
            UserDetail userDetail = null;
            UserDetail InsertedUserDetail = null;
            try
            {   user = MapRegisterInputDTOToUser(registerInputDTO);
                user.Role = Role;

                try
                {
                    InsertedUser = await _userRepo.Add(user);
                }
                catch (DbUpdateException) { throw new UnableToRegisterException("Email ID already exists"); }
                userDetail = MapRegisterInputDTOToUserDetail(registerInputDTO);
                userDetail.UserId = InsertedUser.Id;
                InsertedUserDetail = await _userDetailRepo.Add(userDetail);
                RegisterOutputDTO registerOutputDTO = MapUserToRegisterOutputDTO(InsertedUser);
                return registerOutputDTO;
            }
            catch (UnableToRegisterException) { throw; }
            catch (Exception) { }
            if (InsertedUser == null)
            {
                throw new UnableToRegisterException("Not able to register at this moment");
            }
            if (InsertedUserDetail == null)
            {
                await RevertUserInsert(user);
            }
            throw new UnableToRegisterException("Not able to register at this moment");
        }

        private async Task RevertUserInsert(User user)
        {
            await _userRepo.Delete(user.Id);
        }

        private RegisterOutputDTO MapUserToRegisterOutputDTO(User user)
        {
            RegisterOutputDTO registerOutputDTO = new RegisterOutputDTO();
            registerOutputDTO.Id = user.Id;
            registerOutputDTO.Name = user.Name;
            registerOutputDTO.Phone = user.Phone;
            registerOutputDTO.Age = user.Age;
            registerOutputDTO.Email = user.Email;
            return registerOutputDTO;
        }

        private User MapRegisterInputDTOToUser(RegisterInputDTO registerDTO)
        {
            User user = new User();
            user.Name = registerDTO.Name;
            user.Email = registerDTO.Email;
            user.Phone = registerDTO.Phone;
            user.Age = registerDTO.Age;
            return user;
        }

        private UserDetail MapRegisterInputDTOToUserDetail(RegisterInputDTO registerDTO)
        {
            UserDetail userDetail = new UserDetail();
            userDetail.Status = "Active";

            HMACSHA512 hMACSHA = new HMACSHA512();
            userDetail.PasswordHashKey = hMACSHA.Key;
            userDetail.PasswordEncrypted = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            return userDetail;
        }
    }
}
