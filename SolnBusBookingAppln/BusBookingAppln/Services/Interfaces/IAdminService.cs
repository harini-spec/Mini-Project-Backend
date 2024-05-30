using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Driver;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IAdminService
    {
        #region Summary
        /// <summary>
        /// Register Driver Account, Inactive by default
        /// </summary>
        /// <param name="registerDTO">RegisterDriverInputDTO as input</param>
        /// <returns>RegisterDriverOutputDTO as output</returns>
        #endregion
        public Task<RegisterDriverOutputDTO> RegisterDriver(RegisterDriverInputDTO registerDTO);

        #region Summary
        /// <summary>
        /// Activate Driver account
        /// </summary>
        /// <param name="DriverId">int DriverId as input</param>
        /// <returns>DriverActivateReturnDTO as Output</returns>
        #endregion 
        public Task<DriverActivateReturnDTO> ActivateDriver(int DriverId);
    }
}
