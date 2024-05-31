using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingUnitTest.ServiceUnitTest
{
    public class RewardServiceTest
    {
        IRepository<int, Reward> RewardRepository;

        IRewardService rewardService;

        BusBookingContext context;

        Mock<ILogger<RewardService>> RewardLogger;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("UserAndCustomerDB");
            context = new BusBookingContext(optionsBuilder.Options);

            RewardRepository = new MainRepository<int, Reward>(context);

            RewardLogger = new Mock<ILogger<RewardService>>();

            rewardService = new RewardService(RewardRepository, RewardLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task GetRewardPointsSuccessTest()
        {
            // Arrange 
            await RewardRepository.Add(new Reward
            {
                UserId = 1,
                RewardPoints = 50
            });

            // Action
            var result = await rewardService.GetRewardPoints(1);

            // Assert
            Assert.That(result, Is.EqualTo(50));
        }

        [Test]
        public async Task GetRewardPointsNoRewardsTest()
        {
            // Action
            var result = await rewardService.GetRewardPoints(1);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }
    }
}
