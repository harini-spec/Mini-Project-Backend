using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.TicketDTOs;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;
using System.Net.Sockets;

namespace BusBookingAppln.Services.Classes
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<int, Ticket> _TicketRepository;
        private readonly IRepositoryCompositeKey<int, int, TicketDetail> _TicketDetailRepository;
        private readonly ISeatService _SeatService;
        private readonly ISeatAvailability _SeatAvailabilityService;
        private readonly IScheduleService _ScheduleService;
        private readonly ITransactionService _TransactionService;

        public TicketService(IRepositoryCompositeKey<int, int, TicketDetail> TicketDetailRepository, ISeatAvailability SeatAvailabilityService, IRepository<int, Ticket> TicketRepository, ISeatService seatService, IScheduleService scheduleService, ITransactionService transactionService)
        {
            _TicketRepository = TicketRepository;
            _SeatAvailabilityService = SeatAvailabilityService;
            _ScheduleService = scheduleService;
            _SeatService = seatService;
            _TicketDetailRepository = TicketDetailRepository;
            _TransactionService = transactionService;
        }

        public async Task<bool> CheckIfUserHasActiveTickets(int userId)
        {
            var tickets = new List<Ticket>();
            try
            {
                tickets = await GetAllTickets();
            }
            catch (NoItemsFoundException)
            {
                return false;
            }
            tickets = tickets.ToList().Where(x => x.UserId == userId && x.Status == "Booked").ToList();
            if (tickets.Count == 0)
                return false;
            return true;
        }

        public async Task<string> RemoveTicket(int UserId, int TicketId)
        {
            var ticket = await GetTicketById(TicketId);
            if (ticket.UserId == UserId)
            {
                if (ticket.Status == "Not Booked")
                {
                    await DeleteTicketItems(ticket);
                    await DeleteTicketById(TicketId);
                    return "Ticket Successfully Removed";
                }
                else
                {
                    throw new IncorrectOperationException($"Ticket already {ticket.Status}. Go to the cancellation page");
                }
            }
            throw new UnauthorizedUserException("You can't remove this ticket");
        }

        private async Task DeleteTicketItems(Ticket ticket)
        {
            var ticketDetailsCopy = ticket.TicketDetails.ToList();
            foreach (var ticketItem in ticketDetailsCopy)
            {
                await _TicketDetailRepository.Delete(ticketItem.TicketId, ticketItem.SeatId);
            }
        }

        public async Task<string> UpdateTicketStatusToRideOver(int ScheduleId)
        {
            var tickets = await GetAllTickets();
            foreach (var ticket in tickets)
            {
                if (ticket.ScheduleId == ScheduleId && ticket.Status == "Booked")
                {
                    ticket.Status = "Ride Over";
                    await _TicketRepository.Update(ticket, ticket.Id);
                }
            }
            return "Status successfully updated";
        }

        public async Task<AddedTicketDetailDTO> RemoveTicketItem(int UserId, int TicketId, int SeatId)
        {
            var ticket = await GetTicketById(TicketId);
            if (ticket.UserId == UserId)
            {
                if (ticket.Status == "Not Booked")
                {
                    foreach (var ticketItem in ticket.TicketDetails)
                    {
                        if (ticketItem.SeatId == SeatId)
                        {
                            TicketDetail ticketDetail = new TicketDetail();
                            if (ticket.TicketDetails.Count == 1)
                            {
                                ticketDetail = await _TicketDetailRepository.Delete(TicketId, SeatId);
                                await DeleteTicketById(ticket.Id);
                            }
                            else
                            {
                                ticketDetail = await _TicketDetailRepository.Delete(TicketId, SeatId);
                            }
                            AddedTicketDetailDTO addedTicketDetailDTO = await MapTicketDetailToAddedTicketDetailDTO(ticketDetail);
                            return addedTicketDetailDTO;
                        }
                    }
                    throw new EntityNotFoundException("Ticket Item not found");
                }
                else
                {
                    throw new IncorrectOperationException($"Ticket already {ticket.Status}. Go to the cancellation page");
                }
            }
            throw new UnauthorizedUserException("You can't remove this ticket item");
        }

        public async Task<List<AddedTicketDTO>> GetAllTicketsOfCustomer(int CustomerId)
        {
            var tickets = new List<Ticket>();
            try
            {
                tickets = await GetAllTickets();
                tickets = tickets.Where(x => x.UserId == CustomerId).ToList();
                if (tickets.Count == 0)
                    throw new NoItemsFoundException("Customer has no tickets");
                return await MapTicketsToAddedTicketDTOs(tickets);

            }
            catch (NoItemsFoundException)
            {
                throw;
            }
        }

        public async Task<AddedTicketDTO> AddTicket(int UserId, InputTicketDTO inputTicketDTO)
        {
            Schedule schedule = await _ScheduleService.GetScheduleById(inputTicketDTO.ScheduleId);
            List<int> seatsAvailable = new List<int>();
            List<int> SeatsNotAvailable = new List<int>();

            foreach(var ticketDetail in inputTicketDTO.TicketDetails)
            {
                if(await _SeatAvailabilityService.CheckSeatAvailability(schedule, ticketDetail.SeatId))
                {
                    seatsAvailable.Add(ticketDetail.SeatId);
                }
                else
                {
                    SeatsNotAvailable.Add(ticketDetail.SeatId);
                }
            }

            if (seatsAvailable.Count != inputTicketDTO.TicketDetails.Count)
                throw new NoSeatsAvailableException(SeatsNotAvailable);

            Ticket ticket = new Ticket();
            ticket.UserId = UserId;
            ticket.ScheduleId = schedule.Id;
            ticket.Status = "Not Booked";
            ticket.DateAndTimeOfAdding = DateTime.Now;
            ticket.Total_Cost = await CalculateTicketTotalCost(seatsAvailable);
            ticket.DiscountPercentage = await _TransactionService.CalculateDiscountPercentage(UserId);
            ticket.Final_Amount = _TransactionService.CalculateTicketFinalCost(ticket.Total_Cost, ticket.DiscountPercentage, ticket.GSTPercentage);
            ticket.TicketDetails = await MapInputTicketDetailsToTicketDetails(inputTicketDTO);
            await _TicketRepository.Add(ticket);
            AddedTicketDTO addedTicketDTO = await MapTicketToAddedTicketDTO(ticket);
            return addedTicketDTO;
        }

        private async Task<float> CalculateTicketTotalCost(List<int> seatsAvailable)
        {
            float Total_Cost = 0;
            foreach(int seatId in seatsAvailable)
            {
                Seat seat = await _SeatService.GetSeatById(seatId);
                Total_Cost += seat.SeatPrice;
            }
            return Total_Cost;
        }

        public async Task<List<Ticket>> GetAllTickets()
        {
            return (List<Ticket>)await _TicketRepository.GetAll();
        }

        public async Task<Ticket> GetTicketById(int ticketId)
        {
            return await _TicketRepository.GetById(ticketId);
        }

        public async Task<Ticket> DeleteTicketById(int ticketId)
        {
            return await _TicketRepository.Delete(ticketId);
        }

        // Input TicketDetail List to Model TicketDetail List
        private async Task<List<TicketDetail>> MapInputTicketDetailsToTicketDetails(InputTicketDTO inputTicketDTO)
        {
            List<TicketDetail> ticketDetails = new List<TicketDetail>();
            foreach (var inputTicketDetail in inputTicketDTO.TicketDetails)
            {
                TicketDetail ticketDetail = await MapInputTicketDetailToTicketDetail(inputTicketDetail);
                ticketDetails.Add(ticketDetail);
            }
            return ticketDetails;
        }

        // Input TicketDetail to Model TicketDetail
        private async Task<TicketDetail> MapInputTicketDetailToTicketDetail(InputTicketDetailDTO inputTicketDetail)
        {
            Seat seat = await _SeatService.GetSeatById(inputTicketDetail.SeatId);
            TicketDetail ticketDetail = new TicketDetail();
            ticketDetail.Status = "Not Booked";
            ticketDetail.SeatPrice = seat.SeatPrice;
            ticketDetail.SeatId = inputTicketDetail.SeatId;
            ticketDetail.PassengerName = inputTicketDetail.PassengerName;
            ticketDetail.PassengerGender = inputTicketDetail.PassengerGender;
            ticketDetail.PassengerAge = inputTicketDetail.PassengerAge;
            ticketDetail.PassengerPhone = inputTicketDetail.PassengerPhone;
            return ticketDetail;
        }

        // Model Ticket List to Output Ticket DTO List 
        private async Task<List<AddedTicketDTO>> MapTicketsToAddedTicketDTOs(List<Ticket> tickets)
        {
            List<AddedTicketDTO> result = new List<AddedTicketDTO>();
            foreach (var ticket in tickets)
            {
                AddedTicketDTO addedTicketDTO = await MapTicketToAddedTicketDTO(ticket);
                result.Add(addedTicketDTO);
            }
            return result;
        }

        // Model Ticket to Output Ticket DTO
        private async Task<AddedTicketDTO> MapTicketToAddedTicketDTO(Ticket ticket)
        {
            AddedTicketDTO addedTicketDTO = new AddedTicketDTO();
            addedTicketDTO.TicketId = ticket.Id;
            addedTicketDTO.ScheduleId = ticket.ScheduleId;
            addedTicketDTO.Status = ticket.Status;
            addedTicketDTO.Total_Cost = ticket.Total_Cost;
            addedTicketDTO.DateAndTimeOfAdding = ticket.DateAndTimeOfAdding;
            addedTicketDTO.GSTPercentage = ticket.GSTPercentage;
            addedTicketDTO.DiscountPercentage = ticket.DiscountPercentage;
            addedTicketDTO.Final_Amount = ticket.Final_Amount;
            addedTicketDTO.addedTicketDetailDTOs = await MapTicketDetailListToAddedTicketDetailDTOList(ticket.TicketDetails.ToList());
            return addedTicketDTO;
        }

        // Model TicketDetail List to Output TicketDetail List DTO
        private async Task<List<AddedTicketDetailDTO>> MapTicketDetailListToAddedTicketDetailDTOList(List<TicketDetail> ticketDetails)
        {
            List<AddedTicketDetailDTO> addedTicketDetailDTOs = new List<AddedTicketDetailDTO>();
            foreach(var ticketDetail in ticketDetails)
            {
                addedTicketDetailDTOs.Add(await MapTicketDetailToAddedTicketDetailDTO(ticketDetail));
            }
            return addedTicketDetailDTOs;
        }


        // Model TicketDetail to Output TicketDetail DTO
        private async Task<AddedTicketDetailDTO> MapTicketDetailToAddedTicketDetailDTO(TicketDetail ticketDetail)
        {
            Seat seat = await _SeatService.GetSeatById(ticketDetail.SeatId);
            AddedTicketDetailDTO addedTicketDetailDTO = new AddedTicketDetailDTO();
            addedTicketDetailDTO.SeatId = ticketDetail.SeatId;
            addedTicketDetailDTO.SeatNumber = seat.SeatNumber;
            addedTicketDetailDTO.SeatType = seat.SeatType;
            addedTicketDetailDTO.SeatPrice = ticketDetail.SeatPrice;
            addedTicketDetailDTO.PassengerName = ticketDetail.PassengerName;
            addedTicketDetailDTO.PassengerGender = ticketDetail.PassengerGender;
            addedTicketDetailDTO.PassengerPhone = ticketDetail.PassengerPhone;
            addedTicketDetailDTO.PassengerAge = ticketDetail.PassengerAge;
            addedTicketDetailDTO.Status = ticketDetail.Status;
            return addedTicketDetailDTO;
        }


    }
}
