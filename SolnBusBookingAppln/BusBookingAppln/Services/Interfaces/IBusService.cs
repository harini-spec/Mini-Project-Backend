using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs.Schedule;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IBusService
    {
        #region Summary
        /// <summary>
        /// Add bus with seat details
        /// </summary>
        /// <param name="bus">AddBusDTO as input</param>
        /// <returns>AddBusDTO as output</returns>
        #endregion
        public Task<AddBusDTO> AddBus(AddBusDTO bus);

        #region Summary
        /// <summary>
        /// Get Bus by BusNumber
        /// </summary>
        /// <param name="BusNumber">string BusNumber as input</param>
        /// <returns>Bus Object as output</returns>
        #endregion
        public Task<Bus> GetBusByBusNumber(string BusNumber);

        #region Summary
        /// <summary>
        /// Get all Bus data
        /// </summary>
        /// <returns>List of bus data</returns>
        #endregion
        public Task<List<AddBusDTO>> GetAllBuses();

        #region Summary
        /// <summary>
        /// Checks if a bus is booked in a given time period
        /// </summary>
        /// <param name="schedules">List of Schedule objects as input</param>
        /// <param name="addScheduleDTO">AddScheduleDTO as input</param>
        /// <returns></returns>
        #endregion
        public bool CheckIfBusAlreadyBooked(List<Schedule> schedules, AddScheduleDTO addScheduleDTO);
    }
}
