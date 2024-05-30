using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Schedule;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IScheduleService
    {
        #region Summary 
        /// <summary>
        /// Add schedule for bus and driver
        /// </summary>
        /// <param name="Schedule">Schedule Information DTO</param>
        /// <returns>Added Schedule Information DTO</returns>
        #endregion
        public Task<AddScheduleDTO> AddSchedule(AddScheduleDTO Schedule);

        #region Summary 
        /// <summary>
        /// Get Schedule By Schedule ID
        /// </summary>
        /// <param name="ScheduleId">Id of schedule to be retrieved</param>
        /// <returns>Retrieved Schedule</returns>
        #endregion
        public Task<Schedule> GetScheduleById(int ScheduleId);

        #region Summary 
        /// <summary>
        /// Get all schedules for a given route and date of departure
        /// </summary>
        /// <param name="userInputDTOForSchedule">Route and Date of departure details DTO</param>
        /// <returns>List of schedule information DTOs</returns>
        #endregion
        public Task<List<ScheduleReturnDTO>> GetAllSchedulesForAGivenDateAndRoute(UserInputDTOForSchedule userInputDTOForSchedule);

        #region Summary 
        /// <summary>
        /// Get all schedules
        /// </summary>
        /// <returns>List of schedule information DTOss</returns>
        #endregion
        public Task<List<ScheduleReturnDTO>> GetAllSchedules();

        #region Summary 
        /// <summary>
        /// Get all schedules of a given driver
        /// </summary>
        /// <param name="DriverId">Driver's ID for whom schedules are retrieved</param>
        /// <returns>List of schedule information DTOs</returns>
        #endregion
        public Task<List<ScheduleReturnDTO>> GetAllSchedulesOfDriver(int DriverId);
    }
}
