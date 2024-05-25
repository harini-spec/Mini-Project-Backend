using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class RouteDetailRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("RouteDetailRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }


        [Test]
        public async Task AddRouteDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);

            // Action
            var result = await routeDetailRepository.Add(new RouteDetail
            {
                RouteId = 1,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            });

            // Assert
            Assert.That(result.RouteId, Is.EqualTo(1));

            await routeDetailRepository.Delete(1, 1);
        }

        [Test]
        public async Task AddRouteDetailInvalidOperationExceptionTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);
            var result = await routeDetailRepository.Add(new RouteDetail
            {
                RouteId = 2,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await routeDetailRepository.Add(new RouteDetail
            {
                RouteId = 2,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetRouteDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);
            await routeDetailRepository.Add(new RouteDetail
            {
                RouteId = 3,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            });

            // Action
            var result = await routeDetailRepository.GetById(3, 1);

            // Assert
            Assert.That(result.RouteId, Is.EqualTo(3));

            await routeDetailRepository.Delete(3, 1);
        }

        [Test]
        public async Task GetRouteDetailFailureTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await routeDetailRepository.GetById(100, 3));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type RouteDetail with RouteId = 100 and StopNumber = 3 not found."));
        }

        [Test]
        public async Task DeleteRouteDetailExceptionTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await routeDetailRepository.Delete(100, 3));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type RouteDetail with RouteId = 100 and StopNumber = 3 not found."));
        }

        [Test]
        public async Task GetAllRouteDetailsFailTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);
            await routeDetailRepository.Delete(2, 1);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await routeDetailRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type RouteDetail are found."));
        }

        [Test]
        public async Task GetAllRouteDetailsSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);
            await routeDetailRepository.Add(new RouteDetail
            {
                RouteId = 4,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            });

            // Action
            var result = await routeDetailRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));

            await routeDetailRepository.Delete(4, 1);
        }

        [Test]
        public async Task UpdateRouteDetailExceptionTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);
            var routeDetail = new RouteDetail
            {
                RouteId = 100,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await routeDetailRepository.Update(routeDetail));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type RouteDetail with RouteId = 100 and StopNumber = 1 not found."));
        }

        [Test]
        public async Task DeleteRouteDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);
            await routeDetailRepository.Add(new RouteDetail
            {
                RouteId = 5,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            });

            // Action
            var entity = await routeDetailRepository.Delete(5, 1);

            // Assert
            Assert.That(entity.RouteId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateRouteDetailSuccessTest()
        {
            // Arrange
            IRepositoryCompositeKey<int, int, RouteDetail> routeDetailRepository = new RouteDetailRepository(context);
            await routeDetailRepository.Add(new RouteDetail
            {
                RouteId = 6,
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            });
            var entity = await routeDetailRepository.GetById(6, 1);
            entity.To_Location = "Kerala";

            // Action
            var result = await routeDetailRepository.Update(entity);

            // Assert
            Assert.That(result.To_Location, Is.EqualTo("Kerala"));
        }
    }
}
