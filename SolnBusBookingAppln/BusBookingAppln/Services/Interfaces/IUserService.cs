using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IUserService
    {
        #region Summary 
        /// <summary>
        /// Get user object by Email ID
        /// </summary>
        /// <param name="email">Email ID string</param>
        /// <returns>User with given Email ID</returns>
        #endregion 
        public Task<User> GetUserByEmail(string email);

        #region Summary
        /// <summary>
        /// Register Admin and Customer accounts
        /// </summary>
        /// <param name="registerDTO">Account registration details DTO</param>
        /// <param name="Role">Admin/Customer</param>
        /// <returns>Registered Account details DTO</returns>
        #endregion
        public Task<RegisterOutputDTO> RegisterAdminAndCustomer(RegisterInputDTO registerDTO, string Role);

        #region Summary 
        /// <summary>
        /// Login Admin and Customer with Email and Password
        /// </summary>
        /// <param name="loginDTO">Login details DTO</param>
        /// <returns>Login result DTO with JWT Token</returns>
        #endregion
        public Task<LoginOutputDTO> LoginAdminAndCustomer(LoginInputDTO loginDTO);
    }
}
