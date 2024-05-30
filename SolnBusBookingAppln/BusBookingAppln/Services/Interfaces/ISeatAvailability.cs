using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ISeatAvailability
    {
        #region Summary
        /// <summary>
        /// Deletes tickets which have been not booked for more than an hour
        /// </summary>
        /// <returns>Void Task</returns>
        #endregion
        public Task DeleteNotBookedTickets();

        #region Summary
        /// <summary>
        /// Gets all available seats in the given Schedule
        /// </summary>
        /// <param name="ScheduleId">Id of the schedule to check</param>
        /// <returns>List of Seat DTOs which are available for booking</returns>
        #endregion
        public Task<List<GetSeatsDTO>> GetAllAvailableSeatsInABusSchedule(int ScheduleId);

        #region Summary 
        /// <summary>
        /// Checks if a seat is available for booking and adding to ticket
        /// </summary>
        /// <param name="schedule">Schedule in which seat availability is to be checked</param>
        /// <param name="SeatID">Seat which is to be checked for availability</param>
        /// <returns>Bool value, true if available, else false</returns>
        #endregion
        public Task<bool> CheckSeatAvailability(Schedule schedule, int SeatID);
    }
}
