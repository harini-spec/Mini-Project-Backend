using BusBookingAppln.Contexts;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest
{
    public class BusRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("dummyDB1");
            context = new BusBookingContext(optionsBuilder.Options);
        }

        [Test]
        public async Task AddBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusRepository(context);

            // Action
            var result = await busRepository.Add(new Bus
            {
                BusNumber = "TN04A4444",
                TotalSeats = 30
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetByBusNumberSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusRepository(context);

            // Action
            var result = await busRepository.GetById("TN04A4444");

            // Assert
            Assert.That(result.BusNumber, Is.EqualTo("TN04A4444"));
        }

        [Test]
        public async Task GetAllBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusRepository(context);
            await busRepository.Add(new Bus
            {
                BusNumber = "TN04A5555",
                TotalSeats = 30
            });

            // Action
            var result = await busRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }


        [Test]
        public async Task DeleteBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusRepository(context);

            // Action
            var entity = await busRepository.Delete("TN04A5555");

            // Assert
            Assert.That(entity.BusNumber, Is.EqualTo("TN04A5555"));
        }

        [Test]
        public async Task UpdateBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusRepository(context);
            var entity = await busRepository.GetById("TN04A4444");
            entity.TotalSeats = 15;

            // Action
            var result = await busRepository.Update(entity);

            // Assert
            Assert.That(result.TotalSeats, Is.EqualTo(15));
        }
    }
}
