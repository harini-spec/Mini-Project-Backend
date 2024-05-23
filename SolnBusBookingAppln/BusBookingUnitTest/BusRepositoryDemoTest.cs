using BusBookingAppln.Contexts;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest
{
    public class BusRepositoryDemoTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("dummyDB2");
            context = new BusBookingContext(optionsBuilder.Options);
            context.Add(new Bus
            {
                BusNumber = "TN04A4444",
                TotalSeats = 30
            });
            context.SaveChanges();
        }

        [Test]
        public async Task AddBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusRepository(context);

            // Action
            var result = await busRepository.Add(new Bus
            {
                BusNumber = "TN04A5555",
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
    }
}
//InvalidOperationException
