using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class TicketDetailRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("TicketDetailRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }


        [Test]
        public async Task AddTicketDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);

            // Action
            var result = await ticketDetailRepository.Add(new TicketDetail
            {
                TicketId = 1,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Booked"
            });

            // Assert
            Assert.That(result.TicketId, Is.EqualTo(1));

            await ticketDetailRepository.Delete(1, 1);
        }

        [Test]
        public async Task AddTicketDetailInvalidOperationExceptionTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);
            var result = await ticketDetailRepository.Add(new TicketDetail
            {
                TicketId = 2,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Booked"
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await ticketDetailRepository.Add(new TicketDetail
            {
                TicketId = 2,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Booked"
            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetTicketDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);
            await ticketDetailRepository.Add(new TicketDetail
            {
                TicketId = 3,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Booked"
            });

            // Action
            var result = await ticketDetailRepository.GetById(3, 1);

            // Assert
            Assert.That(result.TicketId, Is.EqualTo(3));

            await ticketDetailRepository.Delete(3, 1);
        }

        [Test]
        public async Task GetTicketDetailFailureTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await ticketDetailRepository.GetById(100, 3));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type TicketDetail with TicketId = 100 and SeatId = 3 not found."));
        }

        [Test]
        public async Task DeleteTicketDetailExceptionTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await ticketDetailRepository.Delete(100, 3));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type TicketDetail with TicketId = 100 and SeatId = 3 not found."));
        }

        [Test]
        public async Task GetAllTicketDetailsFailTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);
            await ticketDetailRepository.Delete(2, 1);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await ticketDetailRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type TicketDetail are found."));
        }

        [Test]
        public async Task GetAllTicketDetailsSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);
            await ticketDetailRepository.Add(new TicketDetail
            {
                TicketId = 4,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Booked"
            });

            // Action
            var result = await ticketDetailRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));

            await ticketDetailRepository.Delete(4, 1);
        }

        [Test]
        public async Task UpdateTicketDetailExceptionTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);
            var ticketDetail = new TicketDetail
            {
                TicketId = 100,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Booked"
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await ticketDetailRepository.Update(ticketDetail));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type TicketDetail with TicketId = 100 and SeatId = 1 not found."));
        }

        [Test]
        public async Task DeleteTicketDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);
            await ticketDetailRepository.Add(new TicketDetail
            {
                TicketId = 5,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Booked"
            });

            // Action
            var entity = await ticketDetailRepository.Delete(5, 1);

            // Assert
            Assert.That(entity.TicketId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateTicketDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, TicketDetail> ticketDetailRepository = new TicketDetailRepository(context);
            await ticketDetailRepository.Add(new TicketDetail
            {
                TicketId = 6,
                SeatId = 1,
                SeatPrice = 500,
                PassengerName = "Ram",
                PassengerAge = 23,
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                Status = "Not Booked"
            });
            var entity = await ticketDetailRepository.GetById(6, 1);
            entity.Status = "Booked";

            // Action
            var result = await ticketDetailRepository.Update(entity);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Booked"));
        }
    }
}
