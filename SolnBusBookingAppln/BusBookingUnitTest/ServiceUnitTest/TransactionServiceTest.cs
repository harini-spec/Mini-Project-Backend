using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs.Transaction;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingUnitTest.ServiceUnitTest
{
    public class TransactionServiceTest
    {
        ISeatAvailability seatAvailability;
        ITransactionService transactionService;

        BusBookingContext context;

        IRepository<string, Payment> PaymentRepository;
        IRepository<string, Refund> RefundRepository;
        IRepository<int, Schedule> ScheduleRepository;
        IRepository<int, Ticket> TicketRepository;
        IRepository<int, Reward> RewardRepository;
        IRepositoryCompositeKey<int, int, TicketDetail> TicketDetailRepository;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("TransactionDB");
            context = new BusBookingContext(optionsBuilder.Options);

            #region Repository Injection
            PaymentRepository = new MainRepository<string, Payment>(context);
            RefundRepository = new MainRepository<string, Refund>(context);
            ScheduleRepository = new MainRepository<int, Schedule>(context);
            TicketRepository = new TicketWithTicketDetailsRepository(context);
            RewardRepository = new MainRepository<int, Reward>(context);
            TicketDetailRepository = new TicketDetailRepository(context);
            #endregion

            #region Service Injection
            seatAvailability = new SeatAvailabilityService(null, null, TicketRepository, TicketDetailRepository);
            transactionService = new TransactionService(seatAvailability, ScheduleRepository, PaymentRepository, RewardRepository, TicketRepository, RefundRepository);
            #endregion

            #region Add Bus and Seats
            Seat seat1 = new Seat()
            {
                Id = 1,
                BusNumber = "TN11A1111",
                SeatNumber = "L1",
                SeatPrice = 50,
                SeatType = "Lower"
            };

            Seat seat2 = new Seat()
            {
                Id = 2,
                BusNumber = "TN11A1111",
                SeatNumber = "U1",
                SeatPrice = 50,
                SeatType = "Upper"
            };
            List<Seat> seats = new List<Seat>() { seat1, seat2 };
            Bus bus = new Bus
            {
                TotalSeats = 2,
                BusNumber = "TN11A1111",
                SeatsInBus = seats
            };
            #endregion

            #region Add Route and RouteDetails
            RouteDetail routedetail = new RouteDetail()
            {
                RouteId = 1,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            };
            List<RouteDetail> routeDetails = new List<RouteDetail> { routedetail };
            Route route = new Route()
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore",
                RouteStops = routeDetails
            };
            #endregion

            #region Add Schedule
            ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11A1111",
                RouteId = 1,
                ScheduledRoute = route,
                DriverId = 1,
                ScheduledBus = bus
            });
            ScheduleRepository.Add(new Schedule
            {
                Id = 2,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now.AddDays(2),
                BusNumber = "TN11A1111",
                RouteId = 1,
                ScheduledRoute = route,
                DriverId = 1,
                ScheduledBus = bus
            });
            #endregion
        }

        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        #region Book Ticket Tests

        [Test, Order(1)]
        public async Task BookTicketSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);

            // Action
            var result = await transactionService.BookTicket(1, 1, "GPay");

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(2)]
        public async Task BookTicketWrongUserIdExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await transactionService.BookTicket(2, 1, "GPay"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You can't book this ticket"));
        }

        [Test, Order(3)]
        public async Task BookTicketWithDiscountSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Reward reward = new Reward()
            {
                UserId = 1,
                RewardPoints = 1000
            };
            User user = new User()
            {
                Id = 1,
                Role = "Customer",
                Name = "Ram",
                RewardsOfUser = reward
            };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                UserBooking = user,
                ScheduleId = 1,
                DiscountPercentage = 10,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);

            // Action
            var result = await transactionService.BookTicket(1, 1, "GPay");

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(4)]
        public async Task BookTicketWithoutDiscountSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked"
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Reward reward = new Reward()
            {
                UserId = 1,
                RewardPoints = 1000
            };
            User user = new User()
            {
                Id = 1,
                Role = "Customer",
                Name = "Ram",
                RewardsOfUser = reward
            };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                UserBooking = user,
                ScheduleId = 1,
                DiscountPercentage = 0,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);

            // Action
            var result = await transactionService.BookTicket(1, 1, "GPay");

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(5)]
        public async Task BookTicketNoTicketFoundExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await transactionService.BookTicket(1, 100, "GPay"));
        }

        [Test, Order(6)]
        public async Task BookTicketTimeLimitExceededExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = Convert.ToDateTime("2024-05-29"),
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);

            // Action
            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await transactionService.BookTicket(1, 1, "GPay"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Time limit to book ticket exceeded"));
        }

        #endregion

        #region Cancel Ticket Tests

        [Test, Order(7)]
        public async Task CancelTicketSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);
            await transactionService.BookTicket(1, 1, "GPay");

            var result = await transactionService.CancelTicket(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(8)]
        public async Task CancelTicketOnDayOfDepartureFailTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);
            await transactionService.BookTicket(1, 1, "GPay");

            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await transactionService.CancelTicket(1, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Can cancel the ticket only 24 hours before the Time of departure"));
        }

        [Test, Order(9)]
        public async Task CancelTicketNotBookedExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 1,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };
            await TicketRepository.Add(ticket);

            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await transactionService.CancelTicket(1, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You can't cancel this ticket: It is not booked/Cancelled/Ride over"));
        }

        [Test, Order(10)]
        public async Task CancelTicketPaymentFailExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };

            await TicketRepository.Add(ticket);
            var result = await transactionService.BookTicket(1, 1, "GPay");

            // Update payment status to fail
            Payment payment = await PaymentRepository.GetById(result.TransactionId);
            payment.Status = "Fail";
            await PaymentRepository.Update(payment, payment.TransactionId);

            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await transactionService.CancelTicket(1, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No payment has been made for this ticket. Cannot process refund"));
        }

        [Test, Order(11)]
        public async Task CancelTicketNoPaymentMadeExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };

            await TicketRepository.Add(ticket);
            var result = await transactionService.BookTicket(1, 1, "GPay");

            Payment payment = await PaymentRepository.GetById(result.TransactionId);
            await PaymentRepository.Delete(payment.TransactionId);

            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await transactionService.CancelTicket(1, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No payment has been made for this ticket. Cannot process refund"));
        }

        [Test, Order(12)]
        public async Task CancelTicketWrongUserExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };

            await TicketRepository.Add(ticket);
            var result = await transactionService.BookTicket(1, 1, "GPay");

            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await transactionService.CancelTicket(2, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You can't cancel this ticket"));
        }
        #endregion

        #region Cancel Seats Tests
        [Test, Order(11)]
        public async Task CancelSeatSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail1 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            TicketDetail ticketDetail2 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 2,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail1, ticketDetail2 };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };

            await TicketRepository.Add(ticket);
            await transactionService.BookTicket(1, 1, "GPay");

            List<int> seatIds = new List<int>(){ 1 };
            CancelSeatsInputDTO cancellationDTO = new CancelSeatsInputDTO()
            {
                SeatIds = seatIds,
                TicketId = 1
            };

            var result = await transactionService.CancelSeats(1, cancellationDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(12)]
        public async Task CancelSeatSeatNotInTicketExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail1 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            TicketDetail ticketDetail2 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 2,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail1, ticketDetail2 };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };

            await TicketRepository.Add(ticket);
            await transactionService.BookTicket(1, 1, "GPay");

            List<int> seatIds = new List<int>() { 3 };
            CancelSeatsInputDTO cancellationDTO = new CancelSeatsInputDTO()
            {
                SeatIds = seatIds,
                TicketId = 1
            };

            // Action
            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await transactionService.CancelSeats(1, cancellationDTO));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("All Seats are not in the Ticket"));
        }

        [Test, Order(13)]
        public async Task CancelSeatAlreadyCancelledExceptionTest()
        {
            // Arrange
            TicketDetail ticketDetail1 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            TicketDetail ticketDetail2 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 2,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail1, ticketDetail2 };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };

            await TicketRepository.Add(ticket);
            await transactionService.BookTicket(1, 1, "GPay");

            List<int> seatIds = new List<int>() { 1 };
            CancelSeatsInputDTO cancellationDTO = new CancelSeatsInputDTO()
            {
                SeatIds = seatIds,
                TicketId = 1
            };
            await transactionService.CancelSeats(1, cancellationDTO);

            // Action
            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await transactionService.CancelSeats(1, cancellationDTO));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You can only cancel Booked seats"));
        }

        [Test, Order(14)]
        public async Task CancelAllSeatsSuccessTest()
        {
            // Arrange
            TicketDetail ticketDetail1 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 1,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            TicketDetail ticketDetail2 = new TicketDetail()
            {
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerAge = 30,
                PassengerPhone = "9999999999",
                SeatId = 2,
                SeatPrice = 50,
                Status = "Not Booked",
            };
            List<TicketDetail> ticketDetails = new List<TicketDetail> { ticketDetail1, ticketDetail2 };
            Ticket ticket = new Ticket()
            {
                Id = 1,
                UserId = 1,
                ScheduleId = 2,
                Status = "Not Booked",
                Total_Cost = 1000,
                DateAndTimeOfAdding = DateTime.Now,
                TicketDetails = ticketDetails
            };

            await TicketRepository.Add(ticket);
            await transactionService.BookTicket(1, 1, "GPay");

            List<int> seatIds = new List<int>() { 1, 2 };
            CancelSeatsInputDTO cancellationDTO = new CancelSeatsInputDTO()
            {
                SeatIds = seatIds,
                TicketId = 1
            };

            // Action
            var result = await transactionService.CancelSeats(1, cancellationDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }
        #endregion
    }
}
