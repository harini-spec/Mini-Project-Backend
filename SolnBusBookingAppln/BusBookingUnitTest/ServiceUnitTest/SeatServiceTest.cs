using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.TicketDTOs;
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
    public class SeatServiceTest
    {
        BusBookingContext context;

        IRepository<int, Seat> seatRepo;
        IRepository<string, Bus> busRepositoryWithSeats;

        ISeatService seatService;

        Mock<ILogger<SeatService>> SeatLogger;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("SeatDB");
            context = new BusBookingContext(optionsBuilder.Options);

            seatRepo = new MainRepository<int, Seat>(context);
            seatRepo.Add(new Seat
            {
                Id = 1,
                BusNumber = "TN11AA1111",
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            });
            busRepositoryWithSeats = new MainRepository<string, Bus>(context);

            SeatLogger = new Mock<ILogger<SeatService>>();

            seatService = new SeatService(seatRepo, SeatLogger.Object, busRepositoryWithSeats);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task GetSeatByIdSuccessTest()
        {
            // Action
            var result = await seatService.GetSeatById(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetSeatByIdExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await seatService.GetSeatById(100));
        }

        [Test]
        public async Task GetSeatsOfBusSuccessTest()
        {
            // Action 
            var seat = new Seat
            {
                Id = 2,
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            };
            await busRepositoryWithSeats.Add(new Bus
            {
                BusNumber = "TN11BB1111",
                TotalSeats = 1,
                SeatsInBus = { seat }
            }) ;
            var result = await seatService.GetSeatsOfBus("TN11BB1111");

            // Assert 
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetSeatsOfBusExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await seatService.GetSeatsOfBus("100"));
        }

        [Test]
        public async Task GetSeatsOfBusNoSeatsFailTest()
        {
            // Action 
            await busRepositoryWithSeats.Add(new Bus
            {
                BusNumber = "TN11CC1111",
                TotalSeats = 0,
            });

            // Assert 
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await seatService.GetSeatsOfBus("TN11CC1111"));
        }

    }
}
