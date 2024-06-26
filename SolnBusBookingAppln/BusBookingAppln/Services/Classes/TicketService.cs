﻿using BusBookingAppln.Exceptions;
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
        private readonly IRewardService _RewardService;
        private readonly IRepositoryCompositeKey<int, int, TicketDetail> _TicketDetailRepository;
        private readonly ISeatService _SeatService;
        private readonly ISeatAvailability _SeatAvailabilityService;
        private readonly IScheduleService _ScheduleService;
        private readonly ILogger<TicketService> _logger;


        public TicketService(IRewardService RewardService, IRepositoryCompositeKey<int, int, TicketDetail> TicketDetailRepository, ISeatAvailability SeatAvailabilityService, IRepository<int, Ticket> TicketRepository, ISeatService seatService, IScheduleService scheduleService, ILogger<TicketService> logger)
        {
            _RewardService = RewardService;
            _TicketRepository = TicketRepository;
            _SeatAvailabilityService = SeatAvailabilityService;
            _ScheduleService = scheduleService;
            _SeatService = seatService;
            _TicketDetailRepository = TicketDetailRepository;
            _logger = logger;
        }


        #region AddTicket

        // Add ticket
        public async Task<TicketReturnDTO> AddTicket(int UserId, InputTicketDTO inputTicketDTO)
        {
            // Check if given schedule is present 
            Schedule schedule = await _ScheduleService.GetScheduleById(inputTicketDTO.ScheduleId);

            List<int> seatsAvailable = new List<int>();
            List<int> SeatsNotAvailable = new List<int>();

            // Check if all the seats user gave are available 
            foreach (var ticketDetail in inputTicketDTO.TicketDetails)
            {
                if (await _SeatAvailabilityService.CheckSeatAvailability(schedule, ticketDetail.SeatId))
                {
                    seatsAvailable.Add(ticketDetail.SeatId);
                }
                else
                {
                    SeatsNotAvailable.Add(ticketDetail.SeatId);
                }
            }

            // Return exception along with unavailable seat IDs
            if (seatsAvailable.Count != inputTicketDTO.TicketDetails.Count)
            {
                _logger.LogError("Some seats are not available: " + string.Join(",", SeatsNotAvailable.ToArray()));
                throw new NoSeatsAvailableException(SeatsNotAvailable);
            }


            // Create Ticket
            Ticket ticket = new Ticket();
            ticket.UserId = UserId;
            ticket.ScheduleId = schedule.Id;
            ticket.Status = "Not Booked";
            ticket.DateAndTimeOfAdding = DateTime.Now;
            ticket.Total_Cost = await CalculateTicketTotalCost(seatsAvailable);
            ticket.DiscountPercentage = await _RewardService.CalculateDiscountPercentage(UserId);
            ticket.Final_Amount = CalculateTicketFinalCost(ticket.Total_Cost, ticket.DiscountPercentage, ticket.GSTPercentage);
            ticket.TicketDetails = await MapInputTicketDetailsToTicketDetails(inputTicketDTO);
            await _TicketRepository.Add(ticket);

            // Map ticket to Output DTO
            TicketReturnDTO addedTicketDTO = await MapTicketToTicketReturnDTO(ticket);
            return addedTicketDTO;
        }

        #endregion


        #region CalculateTicketTotalCost

        // Calculate total ticket cost from seats list
        private async Task<float> CalculateTicketTotalCost(List<int> seatsAvailable)
        {
            float Total_Cost = 0;
            foreach (int seatId in seatsAvailable)
            {
                var seat = await _SeatService.GetSeatById(seatId);
                Total_Cost += seat.SeatPrice;
            }
            return Total_Cost;
        }

        #endregion


        #region UpdateTicketStatusToRideOver

        // Update the status of all tickets with given schedule to 'Ride Over'
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

        #endregion


        #region CheckIfUserHasActiveTickets

        // Checks if user has any Booked tickets where the Ride is not over yet 
        public async Task<bool> CheckIfUserHasActiveTickets(int userId)
        {
            var tickets = new List<Ticket>();
            try
            {
                tickets = await GetAllTickets();
            }
            catch (NoItemsFoundException)
            {
                _logger.LogError("No Tickets in the database");
                return false;
            }
            tickets = tickets.ToList().Where(x => x.UserId == userId && x.Status == "Booked").ToList();
            if (tickets.Count == 0)
                return false;
            return true;
        }

        #endregion


        #region RemoveTicket

        // Remove added ticket
        public async Task<string> RemoveTicket(int UserId, int TicketId)
        {
            var ticket = await GetTicketById(TicketId);

            // Check if its the user's ticket
            if (ticket.UserId == UserId)
            {
                if (ticket.Status == "Not Booked")
                {
                    // Remove all ticket items
                    await DeleteTicketItems(ticket);

                    // Remove ticket
                    await DeleteTicketById(TicketId);
                    return "Ticket Successfully Removed";
                }
                else
                {
                    _logger.LogError($"Ticket status = {ticket.Status}. Wrong action, can't cancel");
                    throw new IncorrectOperationException($"Ticket status = {ticket.Status}. Wrong action, can't cancel");
                }
            }
            _logger.LogError("Wrong user trying to remove the ticket. UserId = " + UserId);
            throw new UnauthorizedUserException("You can't remove this ticket");
        }

        #endregion


        #region DeleteTicketItems

        // Delete all ticket Items of ticket
        private async Task DeleteTicketItems(Ticket ticket)
        {
            var ticketDetailsCopy = ticket.TicketDetails.ToList();
            foreach (var ticketItem in ticketDetailsCopy)
            {
                await _TicketDetailRepository.Delete(ticketItem.TicketId, ticketItem.SeatId);
            }
        }

        #endregion


        #region RemoveTicketItem

        // Remove Ticket Item from Added Ticket
        public async Task<TicketReturnDTO> RemoveTicketItem(int UserId, int TicketId, int SeatId)
        {
            var ticket = await GetTicketById(TicketId);

            // Check if its the user's ticket
            if (ticket.UserId == UserId)
            {
                // Check if the ticket is not booked
                if (ticket.Status == "Not Booked")
                {
                    foreach (var ticketItem in ticket.TicketDetails)
                    {
                        if (ticketItem.SeatId == SeatId)
                        {
                            TicketDetail ticketDetail = new TicketDetail();

                            // If only one item is present, delete the ticket as well
                            if (ticket.TicketDetails.Count == 1)
                            {
                                ticketDetail = await _TicketDetailRepository.Delete(TicketId, SeatId);
                                await DeleteTicketById(ticket.Id);
                                throw new TicketRemovedException("Ticket is removed");
                            }
                            // Else delete only the ticket item
                            else
                            {
                                ticketDetail = await _TicketDetailRepository.Delete(TicketId, SeatId);
                                ticket.Total_Cost -= ticketDetail.SeatPrice;
                                ticket.Final_Amount = CalculateTicketFinalCost(ticket.Total_Cost, ticket.DiscountPercentage, ticket.GSTPercentage);
                                await _TicketRepository.Update(ticket, ticket.Id);
                            }
                            TicketReturnDTO ticketReturnDTO = await MapTicketToTicketReturnDTO(ticket);
                            return ticketReturnDTO;
                        }
                    }
                    _logger.LogError($"Ticket Item with Seat ID = {SeatId} not found");
                    throw new EntityNotFoundException("Ticket Item not found");
                }
                else
                {
                    _logger.LogError($"Ticket Status = {ticket.Status}. Wrong action, Can't cancel");
                    throw new IncorrectOperationException($"Ticket Status = {ticket.Status}. Wrong action, Can't cancel");
                }
            }
            _logger.LogCritical("Wrong user trying to remove the ticket item. UserId = " + UserId);
            throw new UnauthorizedUserException("You can't remove this ticket item");
        }

        #endregion


        #region GetAllTicketsOfCustomer

        // Get all tickets of Customer - Not booked, Booked, Ride over, Cancelled
        public async Task<List<TicketReturnDTO>> GetAllTicketsOfCustomer(int CustomerId)
        {
            var tickets = new List<Ticket>();
            try
            {
                tickets = await GetAllTickets();
                tickets = tickets.Where(x => x.UserId == CustomerId).ToList();
                if (tickets.Count == 0)
                {
                    _logger.LogError("Customer has no tickets");
                    throw new NoItemsFoundException("Customer has no tickets");
                }
                return await MapTicketsToAddedTicketDTOs(tickets);

            }
            catch (NoItemsFoundException)
            {
                _logger.LogError("There are no tickets in the database");
                throw;
            }
        }

        #endregion


        #region CalculateTicketFinalCost

        // Calculate ticket final cost -> total + gst - discount
        public float CalculateTicketFinalCost(float total_Cost, float discountPercentage, float GSTPercentage)
        {
            float GSTAmount = 0;
            float DiscountAmount = 0;
            if (GSTPercentage != 0)
                GSTAmount = total_Cost * GSTPercentage / 100;
            if (discountPercentage != 0)
                DiscountAmount = total_Cost * discountPercentage / 100;
            float finalAmount = total_Cost + GSTAmount - DiscountAmount;
            return finalAmount;
        }

        #endregion


        #region GetAllTickets

        public async Task<List<Ticket>> GetAllTickets()
        {
            _SeatAvailabilityService.DeleteNotBookedTickets();
            return (List<Ticket>)await _TicketRepository.GetAll();
        }

        #endregion


        #region GetTicketById

        public async Task<Ticket> GetTicketById(int ticketId)
        {
            return await _TicketRepository.GetById(ticketId);
        }

        #endregion


        #region DeleteTicketById

        public async Task<Ticket> DeleteTicketById(int ticketId)
        {
            return await _TicketRepository.Delete(ticketId);
        }

        #endregion


        #region Mappers
        // Map InputTicketDTO.TicketDetail List to TicketDetail List
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


        // Input InputTicketDetailDTO to TicketDetail
        private async Task<TicketDetail> MapInputTicketDetailToTicketDetail(InputTicketDetailDTO inputTicketDetail)
        {
            var seat = await _SeatService.GetSeatById(inputTicketDetail.SeatId);
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


        // Map Ticket List to TicketReturnDTO List 
        private async Task<List<TicketReturnDTO>> MapTicketsToAddedTicketDTOs(List<Ticket> tickets)
        {
            List<TicketReturnDTO> result = new List<TicketReturnDTO>();
            foreach (var ticket in tickets)
            {
                TicketReturnDTO addedTicketDTO = await MapTicketToTicketReturnDTO(ticket);
                result.Add(addedTicketDTO);
            }
            return result;
        }


        // Model Ticket to TicketReturnDTO
        private async Task<TicketReturnDTO> MapTicketToTicketReturnDTO(Ticket ticket)
        {
            TicketReturnDTO addedTicketDTO = new TicketReturnDTO();
            addedTicketDTO.TicketId = ticket.Id;
            addedTicketDTO.ScheduleId = ticket.ScheduleId;
            addedTicketDTO.Status = ticket.Status;
            addedTicketDTO.Total_Cost = ticket.Total_Cost;
            addedTicketDTO.DateAndTimeOfAdding = ticket.DateAndTimeOfAdding;
            addedTicketDTO.GSTPercentage = ticket.GSTPercentage;
            addedTicketDTO.DiscountPercentage = ticket.DiscountPercentage;
            addedTicketDTO.Final_Amount = ticket.Final_Amount;
            addedTicketDTO.addedTicketDetailDTOs = await MapTicketDetailListToTicketDetailReturnDTOList(ticket.TicketDetails.ToList());
            return addedTicketDTO;
        }


        // Map TicketDetail List to TicketDetailReturnDTO List 
        private async Task<List<TicketDetailReturnDTO>> MapTicketDetailListToTicketDetailReturnDTOList(List<TicketDetail> ticketDetails)
        {
            List<TicketDetailReturnDTO> addedTicketDetailDTOs = new List<TicketDetailReturnDTO>();
            foreach(var ticketDetail in ticketDetails)
            {
                addedTicketDetailDTOs.Add(await MapTicketDetailToTicketDetailReturnDTO(ticketDetail));
            }
            return addedTicketDetailDTOs;
        }


        // Map TicketDetail to TicketDetailReturnDTO
        private async Task<TicketDetailReturnDTO> MapTicketDetailToTicketDetailReturnDTO(TicketDetail ticketDetail)
        {
            var seat = await _SeatService.GetSeatById(ticketDetail.SeatId);
            TicketDetailReturnDTO addedTicketDetailDTO = new TicketDetailReturnDTO();
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
        #endregion
    }
}
