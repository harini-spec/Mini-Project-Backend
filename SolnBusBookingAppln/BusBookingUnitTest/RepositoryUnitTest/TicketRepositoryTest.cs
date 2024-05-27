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
    public class TicketRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("TicketRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }


        [Test]
        public async Task AddTicketSuccessTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);

            // Action
            var result = await ticketRepository.Add(new Ticket
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000, 
                DateAndTimeOfAdding = DateTime.Now
            });

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));

            await ticketRepository.Delete(1);
        }

        [Test]
        public async Task AddTicketInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);
            var result = await ticketRepository.Add(new Ticket
            {
                Id = 2,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await ticketRepository.Add(new Ticket
            {
                Id = 2,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now
            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByTicketIdSuccessTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);
            await ticketRepository.Add(new Ticket
            {
                Id = 3,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now
            });

            // Action
            var result = await ticketRepository.GetById(3);

            // Assert
            Assert.That(result.Id, Is.EqualTo(3));

            await ticketRepository.Delete(3);
        }

        [Test]
        public async Task GetByTicketIdFailureTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await ticketRepository.GetById(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Ticket with ID 100 not found."));
        }

        [Test]
        public async Task DeleteByTicketIdExceptionTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await ticketRepository.Delete(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Ticket with ID 100 not found."));
        }

        [Test]
        public async Task GetAllTicketFailTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);
            await ticketRepository.Delete(2);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await ticketRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Ticket are found."));
        }

        [Test]
        public async Task GetAllTicketSuccessTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);
            await ticketRepository.Add(new Ticket
            {
                Id = 4,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now
            });

            // Action
            var result = await ticketRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));

            await ticketRepository.Delete(4);
        }

        [Test]
        public async Task UpdateTicketExceptionTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);
            var ticket = new Ticket
            {
                Id = 100,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await ticketRepository.Update(ticket, 100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Ticket with ID 100 not found."));
        }

        [Test]
        public async Task DeleteTicketSuccessTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);
            await ticketRepository.Add(new Ticket
            {
                Id = 5,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now
            });

            // Action
            var entity = await ticketRepository.Delete(5);

            // Assert
            Assert.That(entity.Id, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateTicketSuccessTest()
        {
            // Arrange
            IRepository<int, Ticket> ticketRepository = new TicketWithTicketDetailsRepository(context);
            await ticketRepository.Add(new Ticket
            {
                Id = 6,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now
            });
            var entity = await ticketRepository.GetById(6);
            entity.Status = "Cancelled";

            // Action
            var result = await ticketRepository.Update(entity, 6);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Cancelled"));
        }
    }
}
