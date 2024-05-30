using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class RewardRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("RewardRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task AddRewardSuccessTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);

            // Action
            var result = await RewardRepository.Add(new Reward
            {
                UserId = 1,
                RewardPoints = 100
            });

            // Assert
            Assert.That(result, Is.Not.Null);
            await RewardRepository.Delete(1);
        }

        [Test]
        public async Task AddRewardInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);
            var result = await RewardRepository.Add(new Reward
            {
                UserId = 2,
                RewardPoints = 100
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => RewardRepository.Add(new Reward
            {
                UserId = 2,
                RewardPoints = 100
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByUserIdSuccessTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);
            await RewardRepository.Add(new Reward
            {
                UserId = 3,
                RewardPoints = 100
            });

            // Action
            var result = await RewardRepository.GetById(3);

            // Assert
            Assert.That(result.UserId, Is.EqualTo(3));

            await RewardRepository.Delete(3);
        }

        [Test]
        public async Task GetByUserIdFailureTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RewardRepository.GetById(100));

        }

        [Test]
        public async Task DeleteByUserIdExceptionTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RewardRepository.Delete(100));
        }

        [Test]
        public async Task UpdateByUserIdExceptionTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);
            Reward reward = new Reward
            {
                UserId = 100,
                RewardPoints = 100
            };

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RewardRepository.Update(reward, 100));
        }

        [Test]
        public async Task GetAllRewardsFailTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => RewardRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Reward are found."));
        }

        [Test]
        public async Task GetAllRewardsSuccessTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);
            await RewardRepository.Add(new Reward
            {
                UserId = 4,
                RewardPoints = 100
            });

            // Action
            var result = await RewardRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            await RewardRepository.Delete(4);
        }

        [Test]
        public async Task DeleteRewardSuccessTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);
            await RewardRepository.Add(new Reward
            {
                UserId = 5,
                RewardPoints = 100
            });

            // Action
            var entity = await RewardRepository.Delete(5);

            // Assert
            Assert.That(entity.UserId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateRewardSuccessTest()
        {
            // Arrange
            IRepository<int, Reward> RewardRepository = new MainRepository<int, Reward>(context);
            await RewardRepository.Add(new Reward
            {
                UserId = 6,
                RewardPoints = 100
            });
            var entity = await RewardRepository.GetById(6);
            entity.RewardPoints = 500;

            // Action
            var result = await RewardRepository.Update(entity, 6);

            // Assert
            Assert.That(result.RewardPoints, Is.EqualTo(500));
            await RewardRepository.Delete(6);
        }
    }
}
