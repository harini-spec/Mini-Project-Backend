using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class RouteRepositoryTest
    {
        IRepository<int, Route> RouteRepository;
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("RouteRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
            RouteRepository = new MainRepository<int, Route>(context);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task AddRouteSuccessTest()
        {
            // Action
            var result = await RouteRepository.Add(new Route
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore"
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddRouteInvalidOperationExceptionTest()
        {
            // Arrange
            var result = await RouteRepository.Add(new Route
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore"
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => RouteRepository.Add(new Route
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore"
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByIdSuccessTest()
        {
            // Arrange
            await RouteRepository.Add(new Route
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore"
            });

            // Action
            var result = await RouteRepository.GetById(1);

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdFailureTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RouteRepository.GetById(100));
        }

        [Test]
        public async Task DeleteByIdExceptionTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RouteRepository.Delete(100));
        }

        [Test]
        public async Task UpdateByIdExceptionTest()
        {
            // Arrange
            Route route = new Route
            {
                Id = 100,
                Source = "Chennai",
                Destination = "Bangalore"
            };

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RouteRepository.Update(route, 100));
        }

        [Test]
        public async Task GetAllRoutesFailTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => RouteRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Route are found."));
        }

        [Test]
        public async Task GetAllRoutesSuccessTest()
        {
            // Arrange
            await RouteRepository.Add(new Route
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore"
            });

            // Action
            var result = await RouteRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteRouteSuccessTest()
        {
            // Arrange
            await RouteRepository.Add(new Route
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore"
            });

            // Action
            var entity = await RouteRepository.Delete(1);

            // Assert
            Assert.That(entity.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateRouteSuccessTest()
        {
            // Arrange
            await RouteRepository.Add(new Route
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Bangalore"
            });
            var entity = await RouteRepository.GetById(1);
            entity.Source = "Vellore";

            // Action
            var result = await RouteRepository.Update(entity, 1);

            // Assert
            Assert.That(result.Source, Is.EqualTo("Vellore"));
        }
    }
}
