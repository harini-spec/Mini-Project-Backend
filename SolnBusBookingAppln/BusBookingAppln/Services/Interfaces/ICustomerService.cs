using BusBookingAppln.Models.DTOs.RegisterAndLogin;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ICustomerService
    {
        #region Summary
        /// <summary>
        /// Deletes a Customer Account Temporarily - Changes Status to Inactive
        /// </summary>
        /// <param name="UserId">int UserId as input</param>
        /// <returns>String success message as output</returns>
        #endregion
        public Task<string> SoftDeleteCustomerAccount(int UserId);

        #region Summary
        /// <summary>
        /// Activates Customer Account if username and password are correct - Changes Status to Active
        /// </summary>
        /// <param name="loginInputDTO">LoginInputDTO as input</param>
        /// <returns>LoginOutputDTO as output</returns>
        #endregion
        public Task<LoginOutputDTO> ActivateDeletedCustomerAccount(LoginInputDTO loginInputDTO);
    }
}
