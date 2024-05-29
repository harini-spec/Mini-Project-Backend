using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class BusWithSeatsRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("BusWithSeatsRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }


        [Test]
        public async Task AddBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);

            // Action
            var result = await busRepository.Add(new Bus
            {
                BusNumber = "TN04A1111",
                TotalSeats = 30
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddBusInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);
            await busRepository.Add(new Bus
            {
                BusNumber = "TN04A5555",
                TotalSeats = 30
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => busRepository.Add(new Bus
            {
                BusNumber = "TN04A5555",
                TotalSeats = 30
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByBusNumberSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);
            await busRepository.Add(new Bus
            {
                BusNumber = "TN04A2222",
                TotalSeats = 30
            });

            // Action
            var result = await busRepository.GetById("TN04A2222");

            // Assert
            Assert.That(result.BusNumber, Is.EqualTo("TN04A2222"));
        }

        [Test]
        public async Task GetByBusNumberFailureTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => busRepository.GetById("TN04A0000"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Bus with BusNumber = TN04A0000 not found."));
        }

        [Test]
        public async Task DeleteByBusNumberExceptionTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => busRepository.Delete("TN04A0000"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Bus with BusNumber = TN04A0000 not found."));
        }

        [Test]
        public async Task UpdateByBusNumberExceptionTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);
            var bus = new Bus
            {
                BusNumber = "TN04A0000",
                TotalSeats = 30
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => busRepository.Update(bus, "TN04A0000"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Bus with BusNumber = TN04A0000 not found."));
        }

        [Test]
        public async Task GetAllBusFailTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);
            await busRepository.Delete("TN04A1111");
            await busRepository.Delete("TN04A5555");


            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => busRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Bus are found."));
        }

        [Test]
        public async Task GetAllBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);
            await busRepository.Add(new Bus
            {
                BusNumber = "TN04A4444",
                TotalSeats = 30
            });

            // Action
            var result = await busRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);
            await busRepository.Add(new Bus
            {
                BusNumber = "TN04A3333",
                TotalSeats = 30
            });

            // Action
            var entity = await busRepository.Delete("TN04A3333");

            // Assert
            Assert.That(entity.BusNumber, Is.EqualTo("TN04A3333"));
        }

        [Test]
        public async Task UpdateBusSuccessTest()
        {
            // Arrange
            IRepository<string, Bus> busRepository = new BusWithSeatsRepository(context);
            await busRepository.Add(new Bus
            {
                BusNumber = "TN04A3333",
                TotalSeats = 30
            });
            var entity = await busRepository.GetById("TN04A3333");
            entity.TotalSeats = 15;

            // Action
            var result = await busRepository.Update(entity, "TN04A3333");

            // Assert
            Assert.That(result.TotalSeats, Is.EqualTo(15));
        }
    }
}
