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
    public class SeatAvailabilityServiceTest
    {
        IScheduleService scheduleService;
        IBusService busService;
        ISeatAvailability seatAvailability;

        IRepository<string, Bus> busRepo;
        IRepositoryCompositeKey<int, int, TicketDetail> TicketDetailRepository;
        IRepository<int, Ticket> TicketRepository;
        IRepository<int, Schedule> ScheduleRepository;

        BusBookingContext context;

        Mock<ILogger<BusService>> BusLogger;
        Mock<ILogger<ScheduleService>> ScheduleLogger;
        Mock<ILogger<SeatAvailabilityService>> SeatAvailabilityLogger;


        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("SeatAvailabilityDB");
            context = new BusBookingContext(optionsBuilder.Options);

            TicketRepository = new TicketWithTicketDetailsRepository(context);
            TicketDetailRepository = new TicketDetailRepository(context);
            busRepo = new BusWithSeatsRepository(context);
            ScheduleRepository = new MainRepository<int, Schedule>(context);

            BusLogger = new Mock<ILogger<BusService>>();
            ScheduleLogger = new Mock<ILogger<ScheduleService>>();
            SeatAvailabilityLogger = new Mock<ILogger<SeatAvailabilityService>>();

            busService = new BusService(busRepo, BusLogger.Object);
            scheduleService = new ScheduleService(null, ScheduleRepository, null, null, null, ScheduleLogger.Object);
            seatAvailability = new SeatAvailabilityService(scheduleService, busService, TicketRepository, TicketDetailRepository, SeatAvailabilityLogger.Object);
        }

        #region Check Seat Availability Tests

        [Test, Order(1)]
        public async Task CheckSeatAvailabilityWithNoTicketsReturnsTrueSuccessTest()
        {
            // Arrange
            Seat seat1 = new Seat()
            {
                Id = 1,
                BusNumber = "TN11A1111",
                SeatNumber = "U1",
                SeatPrice = 50,
                SeatType = "Upper"
            };

            Seat seat2 = new Seat()
            {
                Id = 2,
                BusNumber = "TN11A1111",
                SeatNumber = "L1",
                SeatPrice = 50,
                SeatType = "Lower"
            };

            Seat seat3 = new Seat()
            {
                Id = 3,
                BusNumber = "TN11A1111",
                SeatNumber = "U2",
                SeatPrice = 50,
                SeatType = "Upper"
            };

            Seat seat4 = new Seat()
            {
                Id = 4,
                BusNumber = "TN11A1111",
                SeatNumber = "L1",
                SeatPrice = 50,
                SeatType = "Lower"
            };

            List<Seat> seats = new List<Seat>() { seat1, seat2, seat3, seat4 };
            var bus = new Bus()
            {
                BusNumber = "TN11A1111",
                TotalSeats = 4,
                SeatsInBus = seats
            };
            await busRepo.Add(bus);

            Schedule schedule = new Schedule()
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11A1111",
                RouteId = 1,
                DriverId = 1
            };
            await ScheduleRepository.Add(schedule);

            // Action
            var result = await seatAvailability.CheckSeatAvailability(schedule, 1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Order(2)]
        public async Task CheckSeatAvailabilityWithDiffScheduleReturnsTrueSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 1,
                SeatPrice = 50,
                Status = "Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 100,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);
            Schedule schedule = new Schedule()
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11A1111",
                RouteId = 1,
                DriverId = 1
            };

            // Action
            var result = await seatAvailability.CheckSeatAvailability(schedule, 1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Order(3)]
        public async Task CheckSeatAvailabilityWithBookedTicketsReturnsFalseSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 1,
                SeatPrice = 50,
                Status = "Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 2,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);
            Schedule schedule = new Schedule()
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11A1111",
                RouteId = 1,
                DriverId = 1
            };

            // Action
            var result = await seatAvailability.CheckSeatAvailability(schedule, 1);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Order(4)]
        public async Task CheckSeatAvailabilityWithNotBookedTicketsReturnsFalseSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 2,
                SeatPrice = 50,
                Status = "Not Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 3,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);
            Schedule schedule = new Schedule()
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11A1111",
                RouteId = 1,
                DriverId = 1
            };

            // Action
            var result = await seatAvailability.CheckSeatAvailability(schedule, 2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Order(5)]
        public async Task CheckSeatAvailabilityWithNotBookedWithNoSeatMatchReturnsTrueSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 2,
                SeatPrice = 50,
                Status = "Not Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 4,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);
            Schedule schedule = new Schedule()
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11A1111",
                RouteId = 1,
                DriverId = 1
            };

            // Action
            var result = await seatAvailability.CheckSeatAvailability(schedule, 3);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region Delete Not Booked Tickets Tests

        [Test, Order(6)]
        public async Task DeleteNotBookedTicketsSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 3,
                SeatPrice = 50,
                Status = "Not Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 5,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = Convert.ToDateTime("2024-05-24"),
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);
            Schedule schedule = new Schedule()
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11A1111",
                RouteId = 1,
                DriverId = 1
            };

            // Action
            var result = await seatAvailability.CheckSeatAvailability(schedule, 3);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region Get All available seats Tests

        [Test, Order(7)]
        public async Task GetAvailableSeatsInScheduleSuccessTest()
        {

            // Action
            var result = await seatAvailability.GetAllAvailableSeatsInABusSchedule(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test, Order(7)]
        public async Task GetAvailableSeatsInScheduleNoSeatsAvailableExceptionTest()
        {
            // Arrange
            // Arrange
            TicketDetail ticketDetail1 = new TicketDetail()
            {
                SeatId = 3,
                SeatPrice = 50,
                Status = "Booked"
            };
            TicketDetail ticketDetail2 = new TicketDetail()
            {
                SeatId = 4,
                SeatPrice = 50,
                Status = "Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail1, ticketDetail2 };
            Ticket ticket = new Ticket()
            {
                Id = 5,
                UserId = 1,
                ScheduleId = 1,
                Status = "Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);

            // Action
            var exception = Assert.ThrowsAsync<NoSeatsAvailableException>(async () => await seatAvailability.GetAllAvailableSeatsInABusSchedule(1));
        }

        #endregion
    }
}
