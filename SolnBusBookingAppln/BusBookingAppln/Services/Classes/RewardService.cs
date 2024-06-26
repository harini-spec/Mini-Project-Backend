using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Transaction;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;
using System.Net.Sockets;

namespace BusBookingAppln.Services.Classes
{
    public class RewardService : IRewardService
    {
        private readonly IRepository<int, Reward> _RewardRepository; 
        private readonly ILogger<RewardService> _logger;

        public RewardService(IRepository<int, Reward> RewardRepository, ILogger<RewardService> logger)
        {
            _RewardRepository = RewardRepository;
            _logger = logger;
        }


        #region GetRewardPoints

        public async Task<int> GetRewardPoints(int UserId)
        {
            try
            {
                Reward reward = await _RewardRepository.GetById(UserId);
                return reward.RewardPoints;
            }
            catch(EntityNotFoundException enf)
            {
                _logger.LogError(enf.Message);
                return 0;
            }
        }

        #endregion


        #region CalculateDiscountPercentage

        // Calculate discount percentage based on reward points
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
                _logger.LogError($"No Reward record found for User with ID = {userId}");
                return 0;
            }
        }

        #endregion


        #region UpdateRewardPointsForSeatCancellation

        // Reduce reward points provided while booking
        public async Task UpdateRewardPointsForSeatCancellation(int UserId, CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            Reward reward = await _RewardRepository.GetById(UserId);
            reward.RewardPoints -= (10 * cancelSeatsInputDTO.SeatIds.Count());
            await _RewardRepository.Update(reward, UserId);
        }

        #endregion


        #region UpdateRewardPointsForTicketBooking

        // Update reward points for a ticket booking
        public async Task UpdateRewardPointsForTicketBooking(int UserId, Ticket ticket) 
        {
            Reward reward = new Reward();
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
                    reward.RewardPoints += 10 * ticket.TicketDetails.Count();
                    await _RewardRepository.Update(reward, UserId);
                }
            }
            // If it's user's first booking - no reward would be present, so create one
            catch
            {
                _logger.LogError($"No Reward record found for User with ID = {UserId}");
                reward.UserId = UserId;
                reward.RewardPoints = 10 * ticket.TicketDetails.Count();
                await _RewardRepository.Add(reward);
            }
        }

        #endregion

    }
}
