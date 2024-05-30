using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class SeatAvailabilityService : ISeatAvailability
    {
        private readonly IScheduleService _scheduleService;
        private readonly IBusService _busService;
        private readonly IRepositoryCompositeKey<int, int, TicketDetail> _TicketDetailRepository;
        private readonly IRepository<int, Ticket> _TicketRepository;
    

        public SeatAvailabilityService(IScheduleService ScheduleService, IBusService busService, IRepository<int, Ticket> TicketRepository, IRepositoryCompositeKey<int, int, TicketDetail> TicketDetailRepository) 
        {
            _scheduleService = ScheduleService;
            _busService = busService;
            _TicketDetailRepository = TicketDetailRepository;
            _TicketRepository = TicketRepository;
        }


        #region CheckSeatAvailability

        // Check seat availability in a particular schedule - True : Available, False : Not Available
        public async Task<bool> CheckSeatAvailability(Schedule schedule, int SeatID)
        {
            try
            {
                await DeleteNotBookedTickets();

                List<Ticket> tickets = (List<Ticket>)await _TicketRepository.GetAll();

                foreach (Ticket ticket in tickets)
                {
                    // Check if seat is already Booked
                    if (ticket.ScheduleId == schedule.Id && ticket.Status == "Booked")
                    {
                        foreach (var ticketDetail in ticket.TicketDetails)
                        {
                            if (ticketDetail.SeatId == SeatID && ticketDetail.Status == "Booked")
                            {
                                return false;
                            }
                        }
                    }

                    // If it's not been more than an hour from adding the ticket
                    else if(ticket.ScheduleId == schedule.Id && ticket.Status == "Not Booked")
                    {
                        // If seat is already reserved
                        foreach(var ticketDetail in ticket.TicketDetails)
                            if(ticketDetail.SeatId == SeatID)
                                return false;
                    }
                }
                return true;
            }
            catch (NoItemsFoundException) { return true; }
        }

        #endregion


        #region DeleteNotBookedTickets

        // Delete tickets where reservation time limit has exceeded - 1 hr
        public async Task DeleteNotBookedTickets()
        {
            List<Ticket> tickets = (List<Ticket>)await _TicketRepository.GetAll();
            foreach (var ticket in tickets)
            {
                if (ticket.Status == "Not Booked" && ((DateTime.Now - ticket.DateAndTimeOfAdding).TotalHours > 1))
                {
                    var ticketDetailCopy = ticket.TicketDetails.ToList();
                    foreach (var ticketDetail in ticketDetailCopy)
                    {
                        await _TicketDetailRepository.Delete(ticketDetail.TicketId, ticketDetail.SeatId);
                    }
                    await _TicketRepository.Delete(ticket.Id);
                }
            }
        }

        #endregion


        #region GetAllAvailableSeatsInABusSchedule

        // Get all available seats in a schedule
        public async Task<List<GetSeatsDTO>> GetAllAvailableSeatsInABusSchedule(int ScheduleId)
        {
            Schedule schedule = await _scheduleService.GetScheduleById(ScheduleId);
            Bus bus = await _busService.GetBusByBusNumber(schedule.BusNumber);
            List<Seat> seats = bus.SeatsInBus.ToList();
            List<GetSeatsDTO> result = new List<GetSeatsDTO>();
            foreach (var seat in seats)
            {
                if (await CheckSeatAvailability(schedule, seat.Id))
                {
                    GetSeatsDTO getSeatsDTO = MapSeatToGetSeatsDTO(seat);
                    result.Add(getSeatsDTO);
                }
            }
            if (result.Count > 0)
                return result;
            throw new NoSeatsAvailableException();
        }

        #endregion


        #region Mappers

        // Map Seat to GetSeatsDTO
        private GetSeatsDTO MapSeatToGetSeatsDTO(Seat seat)
        {
            GetSeatsDTO getSeatsDTO = new GetSeatsDTO();
            getSeatsDTO.Id = seat.Id;
            getSeatsDTO.SeatNumber = seat.SeatNumber;
            getSeatsDTO.SeatType = seat.SeatType;
            getSeatsDTO.SeatPrice = seat.SeatPrice;
            return getSeatsDTO;
        }

        #endregion
    }
}
