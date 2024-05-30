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
    public class DriverWithScheduleRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("DriverRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task AddDriverSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);

            // Action
            var result = await driverRepository.Add(new Driver
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            });

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task AddDriverInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);
            var result = await driverRepository.Add(new Driver
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await driverRepository.Add(new Driver
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByDriverIdSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);
            await driverRepository.Add(new Driver
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            });

            // Action
            var result = await driverRepository.GetById(1);

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByDriverIdFailureTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await driverRepository.GetById(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Driver with ID 100 not found."));
        }

        [Test]
        public async Task DeleteByDriverIdExceptionTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await driverRepository.Delete(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Driver with ID 100 not found."));
        }

        [Test]
        public async Task GetAllDriverFailTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await driverRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Driver are found."));
        }

        [Test]
        public async Task GetAllDriverSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);
            await driverRepository.Add(new Driver
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            });

            // Action
            var result = await driverRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateDriverExceptionTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);
            var driver = new Driver
            {
                Id = 100,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await driverRepository.Update(driver, 100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Driver with ID 100 not found."));
        }

        [Test]
        public async Task DeleteDriverSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);
            await driverRepository.Add(new Driver
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            });

            // Action
            var entity = await driverRepository.Delete(1);

            // Assert
            Assert.That(entity.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateDriverSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new DriverWithScheduleRepository(context);
            await driverRepository.Add(new Driver
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            });
            var entity = await driverRepository.GetById(1);
            entity.YearsOfExperience = 3;

            // Action
            var result = await driverRepository.Update(entity, 1);

            // Assert
            Assert.That(result.YearsOfExperience, Is.EqualTo(3));
        }
    }
}
