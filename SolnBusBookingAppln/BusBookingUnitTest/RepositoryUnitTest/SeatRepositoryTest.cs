using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class SeatRepositoryTest
    {
        IRepository<int, Seat> SeatRepository;
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("SeatRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
            SeatRepository = new MainRepository<int, Seat>(context);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }


        [Test]
        public async Task AddSeatSuccessTest()
        {
            // Action
            var result = await SeatRepository.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType= "Upper",
                SeatPrice = 50
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddSeatInvalidOperationExceptionTest()
        {
            // Arrange
            var result = await SeatRepository.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => SeatRepository.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByIdSuccessTest()
        {
            // Arrange
            await SeatRepository.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            });

            // Action
            var result = await SeatRepository.GetById(1);

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdFailureTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => SeatRepository.GetById(100));
        }

        [Test]
        public async Task DeleteByIdExceptionTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => SeatRepository.Delete(100));
        }

        [Test]
        public async Task UpdateByIdExceptionTest()
        {
            // Arrange
            Seat seat = new Seat
            {
                Id = 100,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            };

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => SeatRepository.Update(seat, 100));
        }

        [Test]
        public async Task GetAllSeatsFailTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => SeatRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Seat are found."));
        }

        [Test]
        public async Task GetAllSeatsSuccessTest()
        {
            // Arrange
            await SeatRepository.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            });

            // Action
            var result = await SeatRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteSeatsSuccessTest()
        {
            // Arrange
            await SeatRepository.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            });

            // Action
            var entity = await SeatRepository.Delete(1);

            // Assert
            Assert.That(entity.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateSeatSuccessTest()
        {
            // Arrange
            await SeatRepository.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            });
            var entity = await SeatRepository.GetById(1);
            entity.BusNumber = "TN11BB1111";

            // Action
            var result = await SeatRepository.Update(entity, 1);

            // Assert
            Assert.That(result.BusNumber, Is.EqualTo("TN11BB1111"));
        }
    }
}
