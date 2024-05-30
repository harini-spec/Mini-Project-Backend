using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Schedule;
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
    public class ScheduleServiceTest
    {
        IRepository<int, Schedule> ScheduleRepository;
        IRepository<int, Driver> driverRepo;
        IRepository<string, Bus> busRepo;
        IRepository<int, Route> RouteRepo;
        IRepository<int, DriverDetail> DriverDetailRepo;

        BusBookingContext context;

        IScheduleService scheduleService;
        IBusService BusService;
        IRouteService RouteService;
        IDriverService driverService;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("ScheduleRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
            ScheduleRepository = new MainRepository<int, Schedule>(context);
            driverRepo = new DriverWithScheduleRepository(context);
            busRepo = new BusWithSeatsRepository(context);
            RouteRepo = new MainRepository<int, Route>(context);
            DriverDetailRepo = new MainRepository<int, DriverDetail>(context);  

            BusService = new BusService(busRepo);
            RouteService = new RouteService(RouteRepo);
            driverService = new DriverService(driverRepo, null, DriverDetailRepo);

            scheduleService = new ScheduleService(driverRepo, ScheduleRepository, BusService, RouteService, driverService);
        }



        [Test, Order(1)]
        public async Task GetAllSchedulesFailTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await scheduleService.GetAllSchedules());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Schedule are found."));
        }

        [Test, Order(2)]
        public async Task GetAllSchedulesExceptionTest()
        {
            // Arrange
            await ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 200
            });

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await scheduleService.GetAllSchedules());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No Schedules are found"));
        }

        [Test, Order(3)]
        public async Task GetAllSchedulesOfDriverFailTest()
        {
            // Arrange
            Driver driver = new Driver()
            {
                Id = 100,
                Name = "Sam",
                Age = 21,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2,
            };
            await driverRepo.Add(driver);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await scheduleService.GetAllSchedulesOfDriver(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No Schedules are found for Driver with Id 100."));
        }

        [Test, Order(4)]
        public async Task GetScheduleByIdSuccessTest()
        {
            // Arrange
            await RouteRepo.Add(
                new Route
                {
                    Id = 1,
                    Source = "Chennai",
                    Destination = "Karnataka"
                });
            await ScheduleRepository.Add(new Schedule
            {
                Id = 2,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now.AddDays(2),
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 200
            });

            // Action
            var result = await scheduleService.GetScheduleById(2);

            // Assert
            Assert.That(result.Id, Is.EqualTo(2));
        }

        [Test, Order(5)]
        public async Task GetAllSchedulesSuccessTest()
        {
            // Arrange
            await ScheduleRepository.Add(new Schedule
            {
                Id = 3,
                DateTimeOfArrival = DateTime.Now.AddDays(2),
                DateTimeOfDeparture = DateTime.Now.AddDays(2),
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 200
            });

            // Action
            var result = await scheduleService.GetAllSchedules();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2)); // One schedule is today so not given back
        }

        [Test, Order(6)]
        public async Task GetAllSchedulesOfDriverSuccessTest()
        {
            // Arrange
            Driver driver = new Driver()
            {
                Id = 200,
                Name = "Sam",
                Age = 21,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2,
            };
            await driverRepo.Add(driver);

            // Action
            var result = await scheduleService.GetAllSchedulesOfDriver(200);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test, Order(7)]
        public async Task GetAllSchedulesOnAGivenRouteAndDateSuccessTest()
        {
            UserInputDTOForSchedule schedule = new UserInputDTOForSchedule()
            {
                DateTimeOfDeparture = DateTime.Now.AddDays(2),
                Source = "Chennai",
                Destination = "Karnataka"
            };

            // Action
            var result = await scheduleService.GetAllSchedulesForAGivenDateAndRoute(schedule);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test, Order(8)]
        public async Task GetAllSchedulesOnAGivenRouteAndDateExceptionTest()
        {
            // Arrange - Route Added in route service test
            await RouteRepo.Add(
                new Route
                {
                    Id = 2,
                    Source = "Chennai",
                    Destination = "Vellore"
                });
            UserInputDTOForSchedule schedule = new UserInputDTOForSchedule()
            {
                DateTimeOfDeparture = DateTime.Now.AddDays(3),
                Source = "Chennai",
                Destination = "Vellore"
            };

            var exception = Assert.ThrowsAsync<NoSchedulesFoundForGivenRouteAndDate>(async () => await scheduleService.GetAllSchedulesForAGivenDateAndRoute(schedule));
        }

        [Test, Order(9)]
        public async Task AddScheduleBusNotFoundExceptionTest()
        {
            // Arrange 
            AddScheduleDTO addScheduleDTO = new AddScheduleDTO()
            {
                BusNumber = "TN11AA1111",
                DateTimeOfDeparture = DateTime.Now.AddDays(5),
                DateTimeOfArrival = DateTime.Now.AddDays(5),
                RouteId = 1,
                DriverId = 100
            };

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await scheduleService.AddSchedule(addScheduleDTO));

        }

        [Test, Order(10)]
        public async Task AddScheduleSuccessTest()
        {
            // Arrange 
            await busRepo.Add(new Bus
            {
                BusNumber = "TN11AA1111",
                TotalSeats = 2
            });
            AddScheduleDTO addScheduleDTO = new AddScheduleDTO()
            {
                BusNumber = "TN11AA1111",
                DateTimeOfDeparture = DateTime.Now.AddDays(5),
                DateTimeOfArrival = DateTime.Now.AddDays(6),
                RouteId = 1,
                DriverId = 100
            };

            // Action
            var result = await scheduleService.AddSchedule(addScheduleDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(11)]
        public async Task AddScheduleExceptionBusBookedTest()
        {
            // Arrange 
            AddScheduleDTO addScheduleDTO = new AddScheduleDTO()
            {
                BusNumber = "TN11AA1111",
                DateTimeOfDeparture = DateTime.Now.AddDays(5),
                DateTimeOfArrival = DateTime.Now.AddDays(5),
                RouteId = 1,
                DriverId = 200
            };

            var exception = Assert.ThrowsAsync<BusAlreadyBookedException>(async () => await scheduleService.AddSchedule(addScheduleDTO));
        }

        [Test, Order(12)]
        public async Task AddScheduleExceptionDriverBookedTest()
        {
            // Arrange 
            await busRepo.Add(new Bus
            {
                BusNumber = "TN11BB1111",
                TotalSeats = 2
            });
            AddScheduleDTO addScheduleDTO = new AddScheduleDTO()
            {
                BusNumber = "TN11BB1111",
                DateTimeOfDeparture = DateTime.Now.AddDays(5),
                DateTimeOfArrival = DateTime.Now.AddDays(5),
                RouteId = 1,
                DriverId = 100
            };

            var exception = Assert.ThrowsAsync<DriverAlreadyBookedException>(async () => await scheduleService.AddSchedule(addScheduleDTO));
        }

        [Test, Order(13)]
        public async Task AddScheduleSuccessWhenNoScheduleAddedTest()
        {
            // Arrange
            await ScheduleRepository.Delete(1);
            await ScheduleRepository.Delete(2);
            await ScheduleRepository.Delete(3);
            await ScheduleRepository.Delete(4);
            AddScheduleDTO addScheduleDTO = new AddScheduleDTO()
            {
                BusNumber = "TN11AA1111",
                DateTimeOfDeparture = DateTime.Now.AddDays(5),
                DateTimeOfArrival = DateTime.Now.AddDays(6),
                RouteId = 1,
                DriverId = 100
            };

            // Action
            var result = await scheduleService.AddSchedule(addScheduleDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(14)]
        public async Task AddScheduleSuccessDriverAvailableTest()
        {
            // Arrange
            AddScheduleDTO addScheduleDTO = new AddScheduleDTO()
            {
                BusNumber = "TN11AA1111",
                DateTimeOfDeparture = DateTime.Now.AddDays(3),
                DateTimeOfArrival = DateTime.Now.AddDays(4),
                RouteId = 1,
                DriverId = 100
            };

            // Action
            var result = await scheduleService.AddSchedule(addScheduleDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(15)]
        public async Task AddScheduleDriverNotAvailableDueToDepartureTimeExceptionTest()
        {
            // Arrange
            AddScheduleDTO addScheduleDTO = new AddScheduleDTO()
            {
                BusNumber = "TN11BB1111",
                DateTimeOfDeparture = DateTime.Now.AddDays(5),
                DateTimeOfArrival = DateTime.Now.AddDays(7),
                RouteId = 1,
                DriverId = 100
            };

            var exception = Assert.ThrowsAsync<DriverAlreadyBookedException>(async () => await scheduleService.AddSchedule(addScheduleDTO));

        }
    }
}
