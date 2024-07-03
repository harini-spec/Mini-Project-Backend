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
        private readonly ILogger<UserService> _logger;


        public UserService(IRepository<int, User> userRepo, IRepository<int, UserDetail> userDetailRepo, ITokenService tokenService, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _userDetailRepo = userDetailRepo;
            _tokenService = tokenService;
            _logger = logger;
        }


        #region GetUserByEmail

        // Get User object by Email ID
        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                var users = await _userRepo.GetAll();
                var user = users.ToList().FirstOrDefault(x => x.Email == email);
                return user;
            }
            catch(NoItemsFoundException) 
            {
                _logger.LogError($"No user found with given Email ID : {email}");
                return null;
            }
        }

        #endregion  


        #region LoginAdminAndCustomer

        // Login Admin/Customer if their account is active
        public async Task<LoginOutputDTO> LoginAdminAndCustomer(LoginInputDTO loginInputDTO, string Role)
        {
            try
            {
                // Checking if Email ID is present 
                User user = await GetUserByEmail(loginInputDTO.Email);
                if (user == null)
                {
                    _logger.LogError("Email ID not found");
                    throw new UnauthorizedUserException("Invalid username or password");
                }

                if(user.Role != Role)
                {
                    _logger.LogCritical("Unauthorized");
                    throw new UnauthorizedUserException("Invalid username or password");
                }

                UserDetail userDetail = await _userDetailRepo.GetById(user.Id);

                HMACSHA512 hMACSHA = new HMACSHA512(userDetail.PasswordHashKey);
                var encryptedPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginInputDTO.Password));
                bool isPasswordSame = ComparePassword(encryptedPass, userDetail.PasswordEncrypted);

                // Checking if password is correct
                if (isPasswordSame)
                {
                    // Checking if account is active
                    if (userDetail.Status == "Active")
                    {
                        LoginOutputDTO loginOutputDTO = MapUserToLoginReturnDTO(user);
                        return loginOutputDTO;
                    }
                    _logger.LogError("Account is not activated");
                    throw new UserNotActiveException("Your account is not activated");
                }
                _logger.LogError("Wrong password");
                throw new UnauthorizedUserException("Invalid username or password");
            }
            catch (Exception)
            {
                throw;
            }
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

        #endregion


        #region RegisterAdminAndCustomer

        // Register Driver : Status = Active 
        public async Task<RegisterOutputDTO> RegisterAdminAndCustomer(RegisterInputDTO registerInputDTO, string Role)
        {
            User user = null;
            User InsertedUser = null;
            UserDetail userDetail = null;
            UserDetail InsertedUserDetail = null;

            // Checking for duplicate value - Email ID 
            var ExistingUser = await GetUserByEmail(registerInputDTO.Email);
            if (ExistingUser != null)
            {
                _logger.LogError($"Email ID already present: {registerInputDTO.Email}");
                throw new UnableToRegisterException("Email ID already exists");
            }

            try
            {   
                user = MapRegisterInputDTOToUser(registerInputDTO);
                user.Role = Role;
                InsertedUser = await _userRepo.Add(user);
                userDetail = MapRegisterInputDTOToUserDetail(registerInputDTO);
                userDetail.UserId = InsertedUser.Id;
                InsertedUserDetail = await _userDetailRepo.Add(userDetail);
                RegisterOutputDTO registerOutputDTO = MapUserToRegisterOutputDTO(InsertedUser);
                return registerOutputDTO;
            }
            catch (UnableToRegisterException) { throw; }
            catch (Exception) 
            {
                if (InsertedUser == null)
                {
                    _logger.LogError("Not able to register at this moment: User and UserDetail not inserted");
                    throw new UnableToRegisterException("Not able to register at this moment");
                }
                if (InsertedUserDetail == null)
                {
                    _logger.LogError("Not able to register at this moment: UserDetail not inserted");
                    await RevertUserInsert(user);
                    throw new UnableToRegisterException("Not able to register at this moment");
                }
                _logger.LogError("Not able to register at this moment");
                throw new UnableToRegisterException("Not able to register at this moment");
            }
        }


        private async Task RevertUserInsert(User user)
        {
            await _userRepo.Delete(user.Id);
        }

        #endregion


        #region Mappers

        // Map RegisterInputDTO to User
        private User MapRegisterInputDTOToUser(RegisterInputDTO registerDTO)
        {
            User user = new User();
            user.Name = registerDTO.Name;
            user.Email = registerDTO.Email;
            user.Phone = registerDTO.Phone;
            user.Age = registerDTO.Age;
            return user;
        }


        // Map RegisterInputDTO to UserDetail
        private UserDetail MapRegisterInputDTOToUserDetail(RegisterInputDTO registerDTO)
        {
            UserDetail userDetail = new UserDetail();
            userDetail.Status = "Active";

            HMACSHA512 hMACSHA = new HMACSHA512();
            userDetail.PasswordHashKey = hMACSHA.Key;
            userDetail.PasswordEncrypted = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            return userDetail;
        }


        // Map User to RegisterOutputDTO
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


        // Map User to LoginOutputDTO
        private LoginOutputDTO MapUserToLoginReturnDTO(User user)
        {
            LoginOutputDTO loginOutputDTO = new LoginOutputDTO();
            loginOutputDTO.UserID = user.Id;
            loginOutputDTO.UserName = user.Name;
            loginOutputDTO.Token = _tokenService.GenerateToken(user);
            loginOutputDTO.Role = user.Role;
            return loginOutputDTO;
        }

        #endregion

    }
}
