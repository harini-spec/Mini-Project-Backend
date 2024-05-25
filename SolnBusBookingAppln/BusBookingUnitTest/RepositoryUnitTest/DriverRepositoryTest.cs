using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class DriverRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("DriverRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }


        [Test]
        public async Task AddDriverSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);

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

            await driverRepository.Delete(1);
        }

        [Test]
        public async Task AddDriverInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);
            var result = await driverRepository.Add(new Driver
            {
                Id = 2,
                Name = "Ram",
                Age = 25,
                Email = "ram@gmail.com",
                Phone = "9999999988",
                YearsOfExperience = 2

            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await driverRepository.Add(new Driver
            {
                Id = 2,
                Name = "Ram",
                Age = 25,
                Email = "ram@gmail.com",
                Phone = "9999999988",
                YearsOfExperience = 2

            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByDriverIdSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);
            await driverRepository.Add(new Driver
            {
                Id = 3,
                Name = "Ramu",
                Age = 25,
                Email = "ramu@gmail.com",
                Phone = "9999999977",
                YearsOfExperience = 2

            });

            // Action
            var result = await driverRepository.GetById(3);

            // Assert
            Assert.That(result.Id, Is.EqualTo(3));

            await driverRepository.Delete(3);
        }

        [Test]
        public async Task GetByDriverIdFailureTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await driverRepository.GetById(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Driver with ID 100 not found."));
        }

        [Test]
        public async Task DeleteByDriverIdExceptionTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await driverRepository.Delete(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Driver with ID 100 not found."));
        }

        [Test]
        public async Task GetAllDriverFailTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);
            // Used here becuz if used in exception, add in exception is not async so order is changing 
            await driverRepository.Delete(2);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await driverRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Driver are found."));
        }

        [Test]
        public async Task GetAllDriverSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);
            await driverRepository.Add(new Driver
            {
                Id = 4,
                Name = "Ramu",
                Age = 25,
                Email = "ramu@gmail.com",
                Phone = "9999999977",
                YearsOfExperience = 2

            });

            // Action
            var result = await driverRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));

            await driverRepository.Delete(4);
        }

        [Test]
        public async Task UpdateDriverExceptionTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);
            var driver = new Driver
            {
                Id = 100,
                Name = "Ramu",
                Age = 25,
                Email = "ramu@gmail.com",
                Phone = "9999999977",
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
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);
            await driverRepository.Add(new Driver
            {
                Id = 3,
                Name = "Ramu",
                Age = 25,
                Email = "ramu@gmail.com",
                Phone = "9999999977",
                YearsOfExperience = 2
            });

            // Action
            var entity = await driverRepository.Delete(3);

            // Assert
            Assert.That(entity.Id, Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateDriverSuccessTest()
        {
            // Arrange
            IRepository<int, Driver> driverRepository = new MainRepository<int, Driver>(context);
            await driverRepository.Add(new Driver
            {
                Id = 3,
                Name = "Ramu",
                Age = 25,
                Email = "ramu@gmail.com",
                Phone = "9999999977",
                YearsOfExperience = 2
            });
            var entity = await driverRepository.GetById(3);
            entity.YearsOfExperience = 3;

            // Action
            var result = await driverRepository.Update(entity, 3);

            // Assert
            Assert.That(result.YearsOfExperience, Is.EqualTo(3));
        }
    }
}
