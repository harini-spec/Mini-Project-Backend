using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class ScheduleRepositoryTest
    {
        IRepository<int, Schedule> ScheduleRepository;
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("ScheduleRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
            ScheduleRepository = new MainRepository<int, Schedule>(context);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }


        [Test]
        public async Task AddScheduleSuccessTest()
        {
            // Action
            var result = await ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddScheduleInvalidOperationExceptionTest()
        {
            // Arrange
            var result = await ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByIdSuccessTest()
        {
            // Arrange
            await ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            });

            // Action
            var result = await ScheduleRepository.GetById(1);

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdFailureTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => ScheduleRepository.GetById(100));
        }

        [Test]
        public async Task DeleteByIdExceptionTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => ScheduleRepository.Delete(100));
        }

        [Test]
        public async Task UpdateByIdExceptionTest()
        {
            // Arrange
            Schedule schedule = new Schedule
            {
                Id = 100,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            };

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => ScheduleRepository.Update(schedule, 100));
        }

        [Test]
        public async Task GetAllSchedulesFailTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => ScheduleRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Schedule are found."));
        }

        [Test]
        public async Task GetAllSchedulesSuccessTest()
        {
            // Arrange
            await ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            });

            // Action
            var result = await ScheduleRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteSchedulesSuccessTest()
        {
            // Arrange
            await ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            });

            // Action
            var entity = await ScheduleRepository.Delete(1);

            // Assert
            Assert.That(entity.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateScheduleSuccessTest()
        {
            // Arrange
            await ScheduleRepository.Add(new Schedule
            {
                Id = 1,
                DateTimeOfArrival = DateTime.Now,
                DateTimeOfDeparture = DateTime.Now,
                BusNumber = "TN11AA1111",
                RouteId = 1,
                DriverId = 1
            });
            var entity = await ScheduleRepository.GetById(1);
            entity.BusNumber = "TN11BB1111";

            // Action
            var result = await ScheduleRepository.Update(entity, 1);

            // Assert
            Assert.That(result.BusNumber, Is.EqualTo("TN11BB1111"));
        }
    }
}
