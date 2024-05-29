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
        private readonly IRepository<string, Payment> _PaymentRepository;
        private readonly IRepository<string, Refund> _RefundRepository;
        private readonly IRepository<int, Schedule> _ScheduleRepository;
        private readonly IRepository<int, Ticket> _TicketRepository;
        private readonly IRepository<int, Reward> _RewardRepository;
        public TransactionService(IRepository<int, Schedule> ScheduleRepository, IRepository<string, Payment> PaymentRepository, IRepository<int, Reward> RewardRepository, IRepository<int, Ticket> TicketRepository, IRepository<string, Refund> RefundRepository) 
        {
            _PaymentRepository = PaymentRepository;
            _RewardRepository = RewardRepository;
            _TicketRepository = TicketRepository;
            _ScheduleRepository = ScheduleRepository;
            _RefundRepository = RefundRepository;
        }

        public async Task CheckCancellationValidity(int UserId, Ticket ticket)
        {
            if (ticket.Status == "Booked")
            {
                // Check if it's the user's ticket
                if (ticket.UserId != UserId)
                    throw new UnauthorizedUserException("You can't cancel this ticket");

                // Check if its within 24 hrs of Time of departure - No Refund 
                Schedule schedule = await _ScheduleRepository.GetById(ticket.ScheduleId);
                if ((schedule.DateTimeOfDeparture - DateTime.Now).TotalHours <= 24)
                    throw new IncorrectOperationException("Can cancel the ticket only 24 hours before the Time of departure");

                // Can cancel and refund only if the payment is successful 
                try
                {
                    var payments = await _PaymentRepository.GetAll();
                    List<Payment> FinalPayment = payments.Where(x => x.TicketId == ticket.Id && x.Status == "Success").ToList();
                    if (FinalPayment.Count == 0)
                    {
                        throw new IncorrectOperationException("No payment has been made for this ticket. Cannot process refund");
                    }
                }
                catch (Exception)
                {
                    throw new IncorrectOperationException("No payment has been made for this ticket. Cannot process refund");
                }
            }
            else
                throw new IncorrectOperationException("You can't cancel this ticket: It is not booked/Cancelled/Ride over");
        }
            

        public async Task<RefundOutputDTO> CancelTicket(int UserId, int TicketId)
        {
            try
            {
                Ticket ticket = await _TicketRepository.GetById(TicketId);
                await CheckCancellationValidity(UserId, ticket);

                await changeTicketStatus(ticket, "Cancelled");

                Reward reward = await _RewardRepository.GetById(UserId);
                reward.RewardPoints -= (10 * ticket.TicketDetails.Count());
                await _RewardRepository.Update(reward, UserId);

                Refund refund = CreateRefund(ticket, ticket.Total_Cost/2);
                await _RefundRepository.Add(refund);

                RefundOutputDTO refundOutputDTO = MapRefundToRefundOutputDTO(refund);
                return refundOutputDTO;
            }
            catch (Exception) { throw; }
        }

        private RefundOutputDTO MapRefundToRefundOutputDTO(Refund refund)
        {
            RefundOutputDTO refundOutputDTO = new RefundOutputDTO();
            refundOutputDTO.RefundDate = refund.RefundDate;
            refundOutputDTO.RefundAmount = refund.RefundAmount;
            refundOutputDTO.Status = refund.Status;
            refundOutputDTO.TicketId = refund.TicketId;
            return refundOutputDTO;
        }

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

        private async Task changeTicketStatus(Ticket ticket, string status)
        {
            ticket.Status = status;
            foreach (var ticketDetail in ticket.TicketDetails)
            {
                ticketDetail.Status = status;
            }
            await _TicketRepository.Update(ticket, ticket.Id);
        }

        public async Task<PaymentOutputDTO> BookTicket(int UserId, int TicketId, string PaymentMethod)
        {
            Ticket ticket = await _TicketRepository.GetById(TicketId);
            if (ticket.UserId != UserId)
                throw new UnauthorizedUserException("You can't book this ticket");

            Payment payment = CreatePayment(ticket, PaymentMethod);

            Reward reward = null;
            try
            {
                reward = await _RewardRepository.GetById(UserId);
                if (ticket.DiscountPercentage == 10)
                {
                    // If discount provided, 100 pts deducted and 10 points added for every seat
                    reward.RewardPoints -= 100 + (10 * ticket.TicketDetails.Count());
                    await _RewardRepository.Update(reward, UserId);
                }
                else
                {
                    // If no discount is provided, +10 reward pts added for every seat booked 
                    reward.RewardPoints = 10 * ticket.TicketDetails.Count();
                    await _RewardRepository.Update(reward, UserId);
                }
            }
            catch
            {
                reward.UserId = UserId;
                reward.RewardPoints = 10 * ticket.TicketDetails.Count();
                await _RewardRepository.Add(reward);
            }

            await changeTicketStatus(ticket, "Booked");
            await _PaymentRepository.Add(payment);
            PaymentOutputDTO paymentOutputDTO = MapPaymentToPaymentOutputDTO(payment);
            return paymentOutputDTO;
        }

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

        public async Task<float> CalculateDiscountPercentage(int userId)
        {
            Reward reward = null;
            try
            {
                reward = await _RewardRepository.GetById(userId);
                if (reward.RewardPoints >= 100)
                {
                    return 10;
                }
                else
                    return 0;
            }
            catch (EntityNotFoundException)
            {
                return 0;
            }
        }

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

                    float Refund_Amount = CalculateRefundAmount(cancelSeatsInputDTO, ticket);
                    Refund refund = CreateRefund(ticket, Refund_Amount);
                    await _RefundRepository.Add(refund);

                    await UpdateRewardPointsForSeatCancellation(UserId, cancelSeatsInputDTO);
                    await UpdateTicketDetailStatus(cancelSeatsInputDTO);

                    RefundOutputDTO refundOutputDTO = MapRefundToRefundOutputDTO(refund);
                    return refundOutputDTO;
                }
                else
                    throw new IncorrectOperationException("All Seats are not in the Ticket");
            }
            catch(Exception) { throw; }
        }

        private async Task CheckIfSeatAlreadyCancelled(Ticket ticket, CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            foreach (var seatId in cancelSeatsInputDTO.SeatIds)
            {
                // Find the corresponding ticket detail for the seat ID
                var ticketDetail = ticket.TicketDetails.FirstOrDefault(td => td.SeatId == seatId);

                // If ticket detail not found or status is not "booked", return false
                if (ticketDetail == null || ticketDetail.Status != "Booked")
                {
                    throw new IncorrectOperationException("You can only cancel Booked seats");
                }
            }
        }

        private async Task UpdateTicketDetailStatus(CancelSeatsInputDTO cancelSeatsInputDTO)
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

        private async Task CheckIfAllSeatsCancelled(Ticket ticket)
        {
            bool allSeatsCancelled = ticket.TicketDetails.All(ticketDetail => ticketDetail.Status == "Cancelled");
            if(allSeatsCancelled)
                ticket.Status = "Cancelled";
            await _TicketRepository.Update(ticket, ticket.Id);
        }

        private async Task UpdateRewardPointsForSeatCancellation(int UserId, CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            Reward reward = await _RewardRepository.GetById(UserId);
            reward.RewardPoints -= (10 * cancelSeatsInputDTO.SeatIds.Count());
            await _RewardRepository.Update(reward, UserId);
        }

        private float CalculateRefundAmount(CancelSeatsInputDTO cancelSeatsInputDTO, Ticket ticket)
        {
            float refundAmount = cancelSeatsInputDTO.SeatIds
                                .Select(seatId => ticket.TicketDetails.FirstOrDefault(td => td.SeatId == seatId))
                                .Where(ticketDetail => ticketDetail != null) // Filter out null ticket details (for invalid seat IDs)
                                .Sum(ticketDetail => ticketDetail.SeatPrice);
            return refundAmount/2;
        }

        private async Task<bool> CheckIfSeatsInTicket(CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            Ticket ticket = await _TicketRepository.GetById(cancelSeatsInputDTO.TicketId);
            bool allSeatsPresent = cancelSeatsInputDTO.SeatIds.All(seatId => ticket.TicketDetails.Any(td => td.SeatId == seatId));
            return allSeatsPresent;
        }
    }
}
