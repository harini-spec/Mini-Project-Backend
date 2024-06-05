using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.TicketDTOs;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ITicketService
    {
        #region Summary
        /// <summary>
        /// Get Ticket By Id
        /// </summary>
        /// <param name="ticketId">Id of the ticket to be retrieved</param>
        /// <returns>Retrieved ticket</returns>
        #endregion
        public Task<Ticket> GetTicketById(int ticketId);

        #region Summary 
        /// <summary>
        /// Gets all the tickets
        /// </summary>
        /// <returns>List of tickets</returns>
        #endregion
        public Task<List<Ticket>> GetAllTickets();

        #region Summary 
        /// <summary>
        /// Adds a Ticket by checking if the seats are available and calculates final cost
        /// </summary>
        /// <param name="UserId">User adding the ticket</param>
        /// <param name="inputTicketDTO">Input Ticket information DTO</param>
        /// <returns>Added Ticket information DTO</returns>
        #endregion
        public Task<TicketReturnDTO> AddTicket(int UserId, InputTicketDTO inputTicketDTO);

        #region Summary 
        /// <summary>
        /// Delete a ticket by its ID
        /// </summary>
        /// <param name="ticketId">Id of ticket to be deleted</param>
        /// <returns>Deleted ticket</returns>
        #endregion
        public Task<Ticket> DeleteTicketById(int ticketId);

        #region Summary 
        /// <summary>
        /// Removes an added ticket
        /// </summary>
        /// <param name="UserId">User who is removing the ticket</param>
        /// <param name="TicketId">Id of ticket to be removed</param>
        /// <returns>String indicating the result</returns>
        #endregion
        public Task<string> RemoveTicket(int UserId, int TicketId);

        #region Summary 
        /// <summary>
        /// Removes a single ticket item from a ticket
        /// </summary>
        /// <param name="UserId">User removing the item</param>
        /// <param name="TicketId">Ticket from which the item is to be removed</param>
        /// <param name="SeatId">Item Id = Seat Id</param>
        /// <returns>Removed Ticket detail DTO</returns>
        #endregion
        public Task<TicketReturnDTO> RemoveTicketItem(int UserId, int TicketId, int SeatId);

        #region Summary 
        /// <summary>
        /// Updates ticket status to Ride over
        /// </summary>
        /// <param name="ScheduleId">ID of Schedule for which the ride is over</param>
        /// <returns>String message indicating the result of the operation</returns>
        #endregion
        public Task<string> UpdateTicketStatusToRideOver(int ScheduleId);

        #region Summary 
        /// <summary>
        /// Checks if User has active tickets i.e., Booked tickets
        /// </summary>
        /// <param name="userId">Id of user to check active tickets for</param>
        /// <returns>Bool value. True if active tickets present, else false</returns>
        #endregion
        public Task<bool> CheckIfUserHasActiveTickets(int userId);

        #region Summary 
        /// <summary>
        /// Gets all tickets of a customer
        /// </summary>
        /// <param name="CustomerId">ID of the customer for whom tickets are to be retrieved</param>
        /// <returns>List of ticket information DTOs</returns>
        #endregion
        public Task<List<TicketReturnDTO>> GetAllTicketsOfCustomer(int CustomerId);
    }
}
