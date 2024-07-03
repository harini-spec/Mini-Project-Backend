using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
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
    public class BusServiceTest
    {
        BusBookingContext context;

        IRepository<string, Bus> busRepo;
        IBusService busService;

        Mock<ILogger<BusService>> BusLogger;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("BusDB");
            context = new BusBookingContext(optionsBuilder.Options);

            busRepo = new MainRepository<string, Bus>(context);

            BusLogger = new Mock<ILogger<BusService>>();

            busService = new BusService(busRepo, BusLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task GetBusByBusNumberSuccessTest()
        {
            // Arrange
            await busRepo.Add(new Bus
            {
                BusNumber = "TN04A1111",
                TotalSeats = 30
            });

            // Action
            var result = await busService.GetBusByBusNumber("TN04A1111");

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddBusSuccessTest()
        {
            // Arrange
            AddSeatsInputDTO addSeatsInputDTO = new AddSeatsInputDTO() 
            {
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            };
            List<AddSeatsInputDTO> seats = new List<AddSeatsInputDTO> { addSeatsInputDTO };
            AddBusDTO addBusDTO = new AddBusDTO()
            {
                BusNumber = "TN04B1111",
                TotalSeats = 1,
                SeatsInBus = seats
            };

            // Action
            var result = await busService.AddBus(addBusDTO);

            // Assert
            Assert.That(result.BusNumber, Is.EqualTo("TN04B1111"));
        }

        [Test]
        public async Task AddBusFailTest()
        {
            // Arrange
            AddSeatsInputDTO addSeatsInputDTO = new AddSeatsInputDTO()
            {
                SeatNumber = "U1",
                SeatType = "Upper",
                SeatPrice = 50
            };
            List<AddSeatsInputDTO> seats = new List<AddSeatsInputDTO> { addSeatsInputDTO };
            AddBusDTO addBusDTO = new AddBusDTO()
            {
                BusNumber = "TN04C1111",
                TotalSeats = 2,
                SeatsInBus = seats
            };

            // Action
            var exception = Assert.ThrowsAsync<DataDoesNotMatchException>(async () => await busService.AddBus(addBusDTO));
        }

        [Test]
        public async Task GetBusesSuccessTest()
        {
            // Arrange
            await busRepo.Add(new Bus
            {
                BusNumber = "TN04A1111",
                TotalSeats = 30
            });

            // Action
            var result = await busService.GetAllBuses();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetBusesFailTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await busService.GetAllBuses());
        }
    }
}
