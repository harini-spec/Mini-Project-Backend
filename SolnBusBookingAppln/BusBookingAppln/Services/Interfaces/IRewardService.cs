using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Transaction;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IRewardService
    {
        #region Summary
        /// <summary>
        /// Get Reward points of a particular user
        /// </summary>
        /// <param name="UserId">ID of the user to get reward points for</param>
        /// <returns>Reward points in account</returns>
        #endregion
        public Task<int> GetRewardPoints(int UserId);

        #region Summary
        /// <summary>
        /// Updates Reward points after a seat cancellation
        /// </summary>
        /// <param name="UserId">ID of the user for whom reward points are to be updated</param>
        /// <param name="cancelSeatsInputDTO">Seats cancellation information DTO</param>
        /// <returns>Empty Task</returns>
        #endregion
        public Task UpdateRewardPointsForSeatCancellation(int UserId, CancelSeatsInputDTO cancelSeatsInputDTO);

        #region Summary
        /// <summary>
        /// Updates Reward points for ticket booking
        /// </summary>
        /// <param name="UserId">ID of the user for whom reward points are updated</param>
        /// <param name="ticket">Ticket for which reward points are being added</param>
        /// <returns>Empty Task</returns>
        #endregion 
        public Task UpdateRewardPointsForTicketBooking(int UserId, Ticket ticket);

        #region Summary
        /// <summary>
        /// Calculates Discount Percentage based on the Reward points of the user
        /// </summary>
        /// <param name="userId">Id of user for whom discount is being calculated</param>
        /// <returns>Discount value</returns>
        #endregion
        public Task<float> CalculateDiscountPercentage(int userId);

    }
}
