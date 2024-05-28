using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.TicketDTOs;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ITicketService
    {
        public Task<Ticket> GetTicketById(int ticketId);
        public Task<List<Ticket>> GetAllTickets();
        public Task<AddedTicketDTO> AddTicket(int UserId, InputTicketDTO inputTicketDTO);
        public Task<Ticket> DeleteTicketById(int ticketId);
        public Task<string> RemoveTicket(int UserId, int TicketId);
        public Task<AddedTicketDetailDTO> RemoveTicketItem(int UserId, int TicketId, int SeatId);
        public Task<string> UpdateTicketStatusToRideOver(int ScheduleId);
        public Task<bool> CheckIfUserHasActiveTickets(int userId);
        public Task<List<AddedTicketDTO>> GetAllTicketsOfCustomer(int CustomerId);
    }
}
