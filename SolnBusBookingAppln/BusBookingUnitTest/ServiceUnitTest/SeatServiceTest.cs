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

            SeatLogger = new Mock<ILogger<SeatService>>();

            seatService = new SeatService(seatRepo, SeatLogger.Object);
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

    }
}
