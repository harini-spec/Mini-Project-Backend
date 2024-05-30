using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Models.DTOs.Schedule;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IDriverService
    {
        #region Summary
        /// <summary>
        /// Login Driver if username and password are correct
        /// </summary>
        /// <param name="loginDTO">LoginDriverInputDTO as input</param>
        /// <returns>LoginDriverInputDTO as output</returns>
        #endregion
        public Task<LoginDriverOutputDTO> LoginDriver(LoginDriverInputDTO loginDTO);

        #region Summary
        /// <summary>
        /// Change password of Driver account 
        /// </summary>
        /// <param name="email">string Email ID as input</param>
        /// <param name="newPassword">String New Password as output</param>
        /// <returns>Success message as output</returns>
        #endregion
        public Task<string> ChangePassword(string email, string newPassword);

        #region Summary
        /// <summary>
        /// Retrieves a driver by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the driver to retrieve.</param>
        /// <returns>The retrieved driver object</returns>
        #endregion
        public Task<Driver> GetDriverByEmail(string email);

        #region Summary
        /// <summary>
        /// Retrieves a driver by their ID asynchronously.
        /// </summary>
        /// <param name="DriverId">The Id of the driver to retrieve.</param>
        /// <returns>The retrieved driver object</returns>
        #endregion
        public Task<Driver> GetDriverById(int DriverId);

        #region Summary
        /// <summary>
        /// Checks if a driver is available for scheduling based on the provided schedule information and driver ID asynchronously.
        /// </summary>
        /// <param name="addSchedulesDTO">The schedule information to check against.</param>
        /// <param name="driverId">The ID of the driver to check availability for.</param>
        /// <returns>A boolean value indicating whether the driver is available or not</returns>
        #endregion
        public Task<bool> CheckIfDriverAvailable(AddScheduleDTO addSchedulesDTO, int driverId);
    }
}
