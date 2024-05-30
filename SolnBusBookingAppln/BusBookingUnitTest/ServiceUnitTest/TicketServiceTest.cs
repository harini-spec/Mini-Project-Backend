//using BusBookingAppln.Contexts;
//using BusBookingAppln.Models.DBModels;
//using BusBookingAppln.Repositories.Classes;
//using BusBookingAppln.Repositories.Interfaces;
//using BusBookingAppln.Services.Classes;
//using BusBookingAppln.Services.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusBookingUnitTest.ServiceUnitTest
//{
//    public class TicketServiceTest
//    {

//        IRepository<int, Schedule> ScheduleRepository;
//        IRepository<int, Driver> driverRepo;
//        IRepository<string, Bus> busRepo;
//        IRepository<int, Route> RouteRepo;
//        IRepository<int, DriverDetail> DriverDetailRepo;
//        IRepository<int, Ticket> TicketRepository;
//        IRepository<int, Reward> RewardRepository;
//        IRepositoryCompositeKey<int, int, TicketDetail> TicketDetailRepository;

//        BusBookingContext context;

//        ISeatService SeatService;
//        ISeatAvailability SeatAvailabilityService;
//        IScheduleService ScheduleService;
//        ITicketService TicketService;
//        IBusService BusService;
//        IRouteService RouteService;
//        IDriverService driverService;

//        [SetUp]
//        public void Setup()
//        {
//            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("TicketDB");
//            context = new BusBookingContext(optionsBuilder.Options);
//            ScheduleRepository = new MainRepository<int, Schedule>(context);
//            driverRepo = new DriverWithScheduleRepository(context);
//            busRepo = new BusWithSeatsRepository(context);
//            RouteRepo = new MainRepository<int, Route>(context);
//            DriverDetailRepo = new MainRepository<int, DriverDetail>(context);
//            TicketRepository = new TicketWithTicketDetailsRepository(context);
//            RewardRepository = new MainRepository<int, Reward>(context);
//            TicketDetailRepository = new TicketDetailRepository(context);

//            BusService = new BusService(busRepo);
//            RouteService = new RouteService(RouteRepo);
//            driverService = new DriverService(driverRepo, null, DriverDetailRepo);
//            ScheduleService = new ScheduleService(driverRepo, ScheduleRepository, BusService, RouteService, driverService);

//            TicketService = new TicketService(RewardRepository, TicketDetailRepository, SeatAvailabilityService,TicketRepository, ScheduleService);
//        }

//        [Test]
//        public async Task GetBusByBusNumberSuccessTest()
//        {
//            // Arrange
//            await busRepo.Add(new Bus
//            {
//                BusNumber = "TN04A1111",
//                TotalSeats = 30
//            });

//            // Action
//            var result = await busService.GetBusByBusNumber("TN04A1111");

//            // Assert
//            Assert.That(result, Is.Not.Null);
//        }
//    }
//}
