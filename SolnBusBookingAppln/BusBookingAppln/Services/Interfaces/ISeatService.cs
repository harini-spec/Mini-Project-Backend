using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;

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
        public Task<Seat> GetSeatById(int seatId);
    }
}
