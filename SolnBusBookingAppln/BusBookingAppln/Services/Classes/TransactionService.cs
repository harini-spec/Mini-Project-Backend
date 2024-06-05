using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Transaction;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;
using System.Net.Sockets;

namespace BusBookingAppln.Services.Classes
{
    public class TransactionService : ITransactionService
    {
        private readonly ISeatAvailability _SeatAvailability;
        private readonly IRepository<string, Payment> _PaymentRepository;
        private readonly IRepository<string, Refund> _RefundRepository;
        private readonly IRepository<int, Schedule> _ScheduleRepository;
        private readonly IRepository<int, Ticket> _TicketRepository;
        private readonly IRepository<int, Reward> _RewardRepository;
        private readonly ILogger<TransactionService> _logger;
        private readonly IRewardService _rewardService;
        
        public TransactionService(IRewardService rewardService, ISeatAvailability seatAvailability, IRepository<int, Schedule> ScheduleRepository, IRepository<string, Payment> PaymentRepository, IRepository<int, Reward> RewardRepository, IRepository<int, Ticket> TicketRepository, IRepository<string, Refund> RefundRepository, ILogger<TransactionService> logger) 
        {
            _rewardService = rewardService;
            _SeatAvailability = seatAvailability;
            _PaymentRepository = PaymentRepository;
            _RewardRepository = RewardRepository;
            _TicketRepository = TicketRepository;
            _ScheduleRepository = ScheduleRepository;
            _RefundRepository = RefundRepository;
            _logger = logger;
        }

        #region CheckCancellationValidity

        // Checks for the following conditions before cancelling a ticket
        // Check if ticket belongs to the user 
        // Check if ticket is Booked
        // Check if it's not within 24 hrs of the date and time of departure
        // Check if the user's payment for the ticket was successful 
        public async Task CheckCancellationValidity(int UserId, Ticket ticket)
        {
            if (ticket.Status == "Booked")
            {
                // Check if it's the user's ticket
                if (ticket.UserId != UserId)
                {
                    _logger.LogCritical($"Wrong user trying to cancel the ticket. UserId = {UserId}");
                    throw new UnauthorizedUserException("You can't cancel this ticket");
                }

                // Check if its within 24 hrs of Time of departure - No Refund 
                Schedule schedule = await _ScheduleRepository.GetById(ticket.ScheduleId);
                if ((schedule.DateTimeOfDeparture - DateTime.Now).TotalHours <= 24)
                {
                    _logger.LogError("Can cancel the ticket only 24 hours before the Time of departure");
                    throw new IncorrectOperationException("Can cancel the ticket only 24 hours before the Time of departure");
                }

                // Can cancel and refund only if the payment is successful 
                try
                {
                    var payments = await _PaymentRepository.GetAll();
                    List<Payment> FinalPayment = payments.Where(x => x.TicketId == ticket.Id && x.Status == "Success").ToList();
                    if (FinalPayment.Count == 0)
                    {
                        _logger.LogError("No payment has been made for this ticket. Cannot process refund");
                        throw new IncorrectOperationException("No payment has been made for this ticket. Cannot process refund");
                    }
                }
                catch (Exception)
                {
                    _logger.LogError("No payment has been made for this ticket. Cannot process refund");
                    throw new IncorrectOperationException("No payment has been made for this ticket. Cannot process refund");
                }
            }
            else
            {
                _logger.LogError($"You can't cancel this ticket: It's status is {ticket.Status}");
                throw new IncorrectOperationException($"You can't cancel this ticket: It's status is {ticket.Status}");
            }
        }

        #endregion


        #region CancelTicket

        // Cancel Booked ticket
        public async Task<RefundOutputDTO> CancelTicket(int UserId, int TicketId)
        {
            try
            {
                Ticket ticket = await _TicketRepository.GetById(TicketId);
                await CheckCancellationValidity(UserId, ticket);

                // Create refund object -> Refund amount = Total cost of the ticket items / 2
                float amount = CalculateRefundAmount(ticket);

                await changeTicketStatus(ticket, "Cancelled");

                // Reduce provided reward points while booking
                Reward reward = await _RewardRepository.GetById(UserId);
                int ActiveTicketsCount = GetActiveTicketsCount(ticket);
                reward.RewardPoints -= (10 * ActiveTicketsCount);
                await _RewardRepository.Update(reward, UserId);

                Refund refund = CreateRefund(ticket, amount);
                await _RefundRepository.Add(refund);

                RefundOutputDTO refundOutputDTO = MapRefundToRefundOutputDTO(refund);
                return refundOutputDTO;
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        private float CalculateRefundAmount(Ticket ticket)
        {
            float amount = 0;
            foreach(var ticketDetail in ticket.TicketDetails)
            {
                if (ticketDetail.Status == "Booked")
                    amount += ticketDetail.SeatPrice;
            }
            return amount / 2;
        }

        private int GetActiveTicketsCount(Ticket ticket)
        {
            int count = 0;
            foreach(var ticketDetail in ticket.TicketDetails)
            {
                if(ticketDetail.Status == "Booked")
                    count++;
            }
            return count;
        }

        #endregion


        #region CreateRefund

        // Creates refund object

        private Refund CreateRefund(Ticket ticket, float Refund_Amount)
        {
            Refund refund = new Refund();
            refund.TransactionId = Guid.NewGuid().ToString();
            refund.RefundDate = DateTime.Now;
            refund.Status = "Success";
            refund.TicketId = ticket.Id;
            refund.RefundAmount = Refund_Amount;
            return refund;
        }

        #endregion


        #region changeTicketStatus

        // Change ticket status to given status
        private async Task changeTicketStatus(Ticket ticket, string status)
        {
            ticket.Status = status;
            foreach (var ticketDetail in ticket.TicketDetails)
            {
                ticketDetail.Status = status;
            }
            await _TicketRepository.Update(ticket, ticket.Id);
        }

        #endregion


        #region BookTicket

        // Book ticket - Make payment
        public async Task<PaymentOutputDTO> BookTicket(int UserId, int TicketId, string PaymentMethod)
        {
            Ticket ticket = new Ticket();

            // Wrong Ticket Id checking
            try
            {
                ticket = await _TicketRepository.GetById(TicketId);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogError($"Ticket with ID {TicketId} not found");
                throw;
            }

            // Check if its been less than 1 hr of adding
            await _SeatAvailability.DeleteNotBookedTickets();
            try
            {
                ticket = await _TicketRepository.GetById(TicketId);
            }
            catch (EntityNotFoundException)
            // Ticket would've been deleted if it's been more than an hour
            {
                _logger.LogError("It has been more than an hour from the time of adding the ticket");
                throw new IncorrectOperationException("Time limit to book ticket exceeded");
            }

            // Check if ticket belongs to the user
            if (ticket.UserId != UserId)
            {
                _logger.LogCritical("Wrong User trying to book the ticket");
                throw new UnauthorizedUserException("You can't book this ticket");
            }

            if (ticket.Status != "Not Booked")
                throw new IncorrectOperationException("Can't book this ticket");

            Payment payment = CreatePayment(ticket, PaymentMethod);

            await _rewardService.UpdateRewardPointsForTicketBooking(UserId, ticket);

            await changeTicketStatus(ticket, "Booked");
            await _PaymentRepository.Add(payment);
            PaymentOutputDTO paymentOutputDTO = MapPaymentToPaymentOutputDTO(payment);
            return paymentOutputDTO;
        }

        // Create payment object
        private Payment CreatePayment(Ticket ticket, string PaymentMethod)
        {
            Payment payment = new Payment();
            payment.TransactionId = Guid.NewGuid().ToString();
            payment.PaymentDate = DateTime.Now;
            payment.PaymentMethod = PaymentMethod;
            payment.TicketId = ticket.Id;
            payment.AmountPaid = ticket.Final_Amount;
            payment.Status = "Success";
            return payment;
        }

        #endregion


        #region CancelSeats

        // Cancel seats in a booked ticket
        public async Task<RefundOutputDTO> CancelSeats(int UserId, CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            try
            {
                bool allSeatsPresent = await CheckIfSeatsInTicket(cancelSeatsInputDTO);
                if (allSeatsPresent)
                {
                    Ticket ticket = await _TicketRepository.GetById(cancelSeatsInputDTO.TicketId);
                    await CheckCancellationValidity(UserId, ticket);
                    await CheckIfSeatAlreadyCancelled(ticket, cancelSeatsInputDTO);

                    float Refund_Amount = CalculateRefundAmountForCancelledSeats(cancelSeatsInputDTO, ticket);
                    Refund refund = CreateRefund(ticket, Refund_Amount);
                    await _RefundRepository.Add(refund);

                    await _rewardService.UpdateRewardPointsForSeatCancellation(UserId, cancelSeatsInputDTO);
                    await UpdateTicketDetailStatusToCancelled(cancelSeatsInputDTO);

                    RefundOutputDTO refundOutputDTO = MapRefundToRefundOutputDTO(refund);
                    return refundOutputDTO;
                }
                else
                {
                    _logger.LogError("All Seats are not in the Ticket");
                    throw new IncorrectOperationException("All Seats are not in the Ticket");
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        #endregion


        #region CheckIfSeatsInTicket

        // Checks if all seats to be cancelled are in the ticket
        private async Task<bool> CheckIfSeatsInTicket(CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            Ticket ticket = await _TicketRepository.GetById(cancelSeatsInputDTO.TicketId);
            bool allSeatsPresent = cancelSeatsInputDTO.SeatIds.All(seatId => ticket.TicketDetails.Any(td => td.SeatId == seatId));
            return allSeatsPresent;
        }

        #endregion


        #region CheckIfAllSeatsCancelled

        //Checks if all the seats are cancelled. If so, updates ticket status to cancelled
        private async Task CheckIfAllSeatsCancelled(Ticket ticket)
        {
            bool allSeatsCancelled = ticket.TicketDetails.All(ticketDetail => ticketDetail.Status == "Cancelled");
            if (allSeatsCancelled)
                ticket.Status = "Cancelled";
            await _TicketRepository.Update(ticket, ticket.Id);
        }

        #endregion


        #region CheckIfSeatAlreadyCancelled

        //Checks if all seats to be cancelled are booked, and not cancelled
        private async Task CheckIfSeatAlreadyCancelled(Ticket ticket, CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            foreach (var seatId in cancelSeatsInputDTO.SeatIds)
            {
                // Find the corresponding ticket detail for the seat ID
                var ticketDetail = ticket.TicketDetails.FirstOrDefault(td => td.SeatId == seatId);

                // If ticket detail not found or status is not "booked", return false
                if (ticketDetail == null || ticketDetail.Status != "Booked")
                {
                    _logger.LogError("Ticket Seat is Not Booked : You can only cancel Booked seats");
                    throw new IncorrectOperationException("You can only cancel Booked seats");
                }
            }
        }

        #endregion


        #region UpdateTicketDetailStatusToCancelled

        // Update TicketDetails Status to Cancelled
        private async Task UpdateTicketDetailStatusToCancelled(CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            Ticket ticket = await _TicketRepository.GetById(cancelSeatsInputDTO.TicketId);
            foreach (var seatId in cancelSeatsInputDTO.SeatIds)
            {
                var ticketDetail = ticket.TicketDetails.FirstOrDefault(td => td.SeatId == seatId);
                if (ticketDetail != null)
                {
                    ticketDetail.Status = "Cancelled";
                }
            }
            await _TicketRepository.Update(ticket, ticket.Id);
            await CheckIfAllSeatsCancelled(ticket);
        }

        #endregion


        #region CalculateRefundAmountForCancelledSeats

        // Refund amount for cancelled seats - Refund amount = add seat price and then divide by 2 
        private float CalculateRefundAmountForCancelledSeats(CancelSeatsInputDTO cancelSeatsInputDTO, Ticket ticket)
        {
            float refundAmount = cancelSeatsInputDTO.SeatIds
                                .Select(seatId => ticket.TicketDetails.FirstOrDefault(td => td.SeatId == seatId))
                                .Where(ticketDetail => ticketDetail != null) // Filter out null ticket details (for invalid seat IDs)
                                .Sum(ticketDetail => ticketDetail.SeatPrice);
            return refundAmount/2;
        }

        #endregion


        #region Mappers

        // Map Refund to RefundOutputDTO
        private RefundOutputDTO MapRefundToRefundOutputDTO(Refund refund)
        {
            RefundOutputDTO refundOutputDTO = new RefundOutputDTO();
            refundOutputDTO.RefundDate = refund.RefundDate;
            refundOutputDTO.RefundAmount = refund.RefundAmount;
            refundOutputDTO.Status = refund.Status;
            refundOutputDTO.TicketId = refund.TicketId;
            return refundOutputDTO;
        }

        // Map Payment to PaymentOutputDTO
        private PaymentOutputDTO MapPaymentToPaymentOutputDTO(Payment payment)
        {
            PaymentOutputDTO paymentOutputDTO = new PaymentOutputDTO();
            paymentOutputDTO.TransactionId = payment.TransactionId;
            paymentOutputDTO.PaymentMethod = payment.PaymentMethod;
            paymentOutputDTO.PaymentDate = payment.PaymentDate;
            paymentOutputDTO.AmountPaid = payment.AmountPaid;
            paymentOutputDTO.Status = payment.Status;
            return paymentOutputDTO;
        }

        #endregion

    }
}
