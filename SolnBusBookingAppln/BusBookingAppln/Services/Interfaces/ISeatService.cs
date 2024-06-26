using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.Bus;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ISeatService
    {
        #region Summary
        /// <summary>
        /// Get seat by SeatID
        /// </summary>
        /// <param name="seatId">ID of the seat to be retrieved</param>
        /// <returns>Seat retrieved</returns>
        #endregion 
        public Task<GetSeatsDTO> GetSeatById(int seatId);

        #region Summary
        /// <summary>
        /// Gets all the seats in a Bus
        /// </summary>
        /// <param name="BusNumber">Number plate of the bus</param>
        /// <returns>All the seat data of the bus</returns>
        #endregion
        public Task<List<GetSeatsDTO>> GetSeatsOfBus(string BusNumber);
    }
}
