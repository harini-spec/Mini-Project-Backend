using BusBookingAppln.Contexts;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.TicketDTOs;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Exceptions;

namespace BusBookingUnitTest.ServiceUnitTest
{
    public class TicketServiceTest
    {
        IRepository<int, Ticket> TicketRepository;
        IRepository<int, Reward> RewardRepository;
        IRepositoryCompositeKey<int, int, TicketDetail> TicketDetailRepository;
        IRepository<int, Seat> SeatRepository;
        IRepository<string, Bus> busRepo;
        IRepository<int, Schedule> ScheduleRepository;
        IRepository<int, Driver> driverRepo;
        IRepository<int, BusBookingAppln.Models.DBModels.Route> RouteRepo;
        IRepository<int, DriverDetail> DriverDetailRepo;

        BusBookingContext context;

        IBusService BusService;
        IRouteService RouteService;
        IDriverService driverService;
        IScheduleService scheduleService;
        ISeatService seatService;
        ISeatAvailability seatAvailabilityService;

        ITicketService ticketService;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("TicketDB");
            context = new BusBookingContext(optionsBuilder.Options);

            #region Repository Injection
            TicketRepository = new TicketWithTicketDetailsRepository(context);
            RewardRepository = new MainRepository<int, Reward>(context);
            TicketDetailRepository = new TicketDetailRepository(context);
            SeatRepository = new MainRepository<int, Seat>(context);
            busRepo = new BusWithSeatsRepository(context);
            ScheduleRepository = new MainRepository<int, Schedule>(context);
            driverRepo = new DriverWithScheduleRepository(context);
            RouteRepo = new MainRepository<int, BusBookingAppln.Models.DBModels.Route>(context);
            DriverDetailRepo = new MainRepository<int, DriverDetail>(context);
            #endregion

            #region Service Injection
            BusService = new BusService(busRepo);
            RouteService = new RouteService(RouteRepo);
            driverService = new DriverService(driverRepo, null, DriverDetailRepo);
            scheduleService = new ScheduleService(driverRepo, ScheduleRepository, BusService, RouteService, driverService);
            seatService = new SeatService(SeatRepository);
            seatAvailabilityService = new SeatAvailabilityService(scheduleService, BusService, TicketRepository, TicketDetailRepository);

            ticketService = new TicketService(RewardRepository, TicketDetailRepository, seatAvailabilityService, TicketRepository, seatService, scheduleService);
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
            BusBookingAppln.Models.DBModels.Route route = new BusBookingAppln.Models.DBModels.Route()
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

        #region Add Ticket Tests
        [Test]
        public async Task AddTicketSuccessTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List <InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            
            // Action
            var result = await ticketService.AddTicket(1, inputTicketDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddTicketWithEnoughRewardPointsSuccessTest()
        {
            // Arrange
            Reward reward = new Reward
            {
                UserId = 2,
                RewardPoints = 100
            };
            await RewardRepository.Add(reward);
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };

            // Action
            var result = await ticketService.AddTicket(2, inputTicketDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddTicketWithLessRewardPointsSuccessTest()
        {
            // Arrange
            Reward reward = new Reward
            {
                UserId = 2,
                RewardPoints = 50
            };
            await RewardRepository.Add(reward);
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };

            // Action
            var result = await ticketService.AddTicket(2, inputTicketDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddTicketAlreadyBookedFailTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);

            // Action
            var exception = Assert.ThrowsAsync<NoSeatsAvailableException>(async () => await ticketService.AddTicket(1, inputTicketDTO));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Following seats are not available: 1"));
        }

        #endregion

        #region Remove Ticket Tests
        [Test]
        public async Task RemoveTicketSuccessTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);

            // Action 
            var result = await ticketService.RemoveTicket(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task RemoveTicketWrongUserExceptionTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);

            // Action 
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await ticketService.RemoveTicket(2, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You can't remove this ticket"));
        }

        [Test]
        public async Task RemoveBookedTicketExceptionTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };

            // Change Status to Booked
            await ticketService.AddTicket(1, inputTicketDTO);
            var ticket = await ticketService.GetTicketById(1);
            ticket.Status = "Booked";
            await TicketRepository.Update(ticket, ticket.Id);
            var ticketdetail = await TicketDetailRepository.GetById(1, 1);
            ticketdetail.Status = "Booked";
            await TicketDetailRepository.Update(ticketdetail);

            // Action 
            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await ticketService.RemoveTicket(1, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Ticket already Booked. Go to the cancellation page"));
        }
        #endregion

        #region Update Ticket Status to Ride Over Tests

        [Test]
        public async Task UpdateTicketStatusToRideOverSuccessTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);


            // Change Status to Booked
            var ticket = await ticketService.GetTicketById(1);
            ticket.Status = "Booked";
            await TicketRepository.Update(ticket, ticket.Id);
            var ticketdetail = await TicketDetailRepository.GetById(1, 1);
            ticketdetail.Status = "Booked";
            await TicketDetailRepository.Update(ticketdetail);


            // Action 
            var result = await ticketService.UpdateTicketStatusToRideOver(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task UpdateTicketStatusToRideOverNoTicketsTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);

            // Action 
            var result = await ticketService.UpdateTicketStatusToRideOver(100);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task UpdateTicketStatusToRideOverNotBookedTicketTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);


            // Action 
            var result = await ticketService.UpdateTicketStatusToRideOver(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task UpdateTicketStatusToRideOverBookedTicketTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);


            // Change Status to Booked
            var ticket = await ticketService.GetTicketById(1);
            ticket.Status = "Booked";
            await TicketRepository.Update(ticket, ticket.Id);
            var ticketdetail = await TicketDetailRepository.GetById(1, 1);
            ticketdetail.Status = "Booked";
            await TicketDetailRepository.Update(ticketdetail);


            // Action 
            var result = await ticketService.UpdateTicketStatusToRideOver(2);

            // Assert
            Assert.That(result, Is.Not.Null);
        }
        #endregion

        #region Remove Ticket Item Tests


        [Test]
        public async Task RemoveTicketItemWithOneItemInTicketSuccessTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);


            // Action 
            var result = await ticketService.RemoveTicketItem(1, 1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task RemoveTicketItemWrongUserExceptionTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);

            // Action 
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await ticketService.RemoveTicketItem(2, 1, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You can't remove this ticket item"));
        }

        [Test]
        public async Task RemoveBookedTicketItemExceptionTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };

            // Change Status to Booked
            await ticketService.AddTicket(1, inputTicketDTO);
            var ticket = await ticketService.GetTicketById(1);
            ticket.Status = "Booked";
            await TicketRepository.Update(ticket, ticket.Id);
            var ticketdetail = await TicketDetailRepository.GetById(1, 1);
            ticketdetail.Status = "Booked";
            await TicketDetailRepository.Update(ticketdetail);

            // Action 
            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await ticketService.RemoveTicketItem(1, 1, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Ticket already Booked. Go to the cancellation page"));
        }

        [Test]
        public async Task RemoveTicketItemTicketItemNotFoundExceptionTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };

            await ticketService.AddTicket(1, inputTicketDTO);

            // Action 
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await ticketService.RemoveTicketItem(1, 1, 2));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Ticket Item not found"));
        }

        [Test]
        public async Task RemoveTicketItemWithManyItemsInTicketSuccessTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO1 = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            InputTicketDetailDTO ticketDetailDTO2 = new InputTicketDetailDTO()
            {
                SeatId = 2,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO1, ticketDetailDTO2 };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };

            await ticketService.AddTicket(1, inputTicketDTO);

            // Action 
            var result = await ticketService.RemoveTicketItem(1, 1, 2);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        #endregion

        #region Get All Tickets of Customer Tests

        [Test]
        public async Task GetAllTicketsOfCustomerSuccessTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);

            // Action
            var result = await ticketService.GetAllTicketsOfCustomer(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllTicketsOfCustomerNoCustomerTicketsTest()
        {
            // Arrange
            InputTicketDetailDTO ticketDetailDTO = new InputTicketDetailDTO()
            {
                SeatId = 1,
                PassengerName = "Sam",
                PassengerGender = "Male",
                PassengerPhone = "9999999999",
                PassengerAge = 25
            };
            List<InputTicketDetailDTO> inputTicketDetailDTOs = new List<InputTicketDetailDTO>() { ticketDetailDTO };
            InputTicketDTO inputTicketDTO = new InputTicketDTO()
            {
                ScheduleId = 1,
                TicketDetails = inputTicketDetailDTOs
            };
            await ticketService.AddTicket(1, inputTicketDTO);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await ticketService.GetAllTicketsOfCustomer(2));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Customer has no tickets"));
        }

        [Test]
        public async Task GetAllTicketsOfCustomerNoTicketsTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await ticketService.GetAllTicketsOfCustomer(1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Ticket are found."));
        }

        #endregion
    }
}
