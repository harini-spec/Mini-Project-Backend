﻿using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs.Route;
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
    public class RouteServiceTest
    {
        IRepository<int, Route> RouteRepository;

        BusBookingContext context;

        IRouteService routeService;

        Mock<ILogger<RouteService>> RouteLogger;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("RouteDB");
            context = new BusBookingContext(optionsBuilder.Options);

            RouteRepository = new RouteWithRouteDetailRepository(context);

            RouteLogger = new Mock<ILogger<RouteService>>();

            routeService = new RouteService(RouteRepository, RouteLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        #region Get Route Tests

        [Test, Order(1)]
        public async Task GetRouteBySourceAndDestExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await routeService.GetRoute("Chennai", "Vellore"));
        }

        [Test, Order(2)]
        public async Task GetRouteBySourceAndDestDestNotFoundFailTest()
        {
            // Arrange
            Route route = new Route()
            {
                Id = 1,
                Source = "Chennai",
                Destination = "Vellore"
            };
            var result = await RouteRepository.Add(route);

            // Action
            var exception = Assert.ThrowsAsync<NoRoutesFoundForGivenSourceAndDest>(async () => await routeService.GetRoute("Chennai", "Bangalore"));
        }

        [Test, Order(3)]
        public async Task GetRouteBySourceAndDestSourceNotFoundFailTest()
        {
            // Arrange
            Route route = new Route()
            {
                Id = 100,
                Source = "Chennai",
                Destination = "Vellore"
            };
            var result = await RouteRepository.Add(route);

            // Action
            var exception = Assert.ThrowsAsync<NoRoutesFoundForGivenSourceAndDest>(async () => await routeService.GetRoute("Bangalore", "Chennai"));
        }

        [Test, Order(4)]
        public async Task GetRouteByIdSuccessTest()
        {
            // Arrange
            Route route = new Route()
            {
                Id = 2,
                Source = "Chennai",
                Destination = "Vellore"
            };
            var result = await RouteRepository.Add(route);

            // Action
            await routeService.GetRoute(result.Id);

            // Assert
            Assert.That(result.Id, Is.EqualTo(2));
        }

        [Test, Order(5)]
        public async Task GetRouteBySourceAndDestSuccessTest()
        {
            // Arrange
            Route route = new Route()
            {
                Id = 3,
                Source = "Chennai",
                Destination = "Vellore"
            };
            var result = await RouteRepository.Add(route);

            // Action
            await routeService.GetRoute(route.Source, route.Destination);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Source, Is.EqualTo("Chennai"));
                Assert.That(result.Destination, Is.EqualTo("Vellore"));
            });
        }

        #endregion


        #region Add Route Tests

        [Test, Order(4)]
        public async Task AddRouteBySourceAndDestSuccessTest()
        {
            // Arrange
            RouteDetailsDTO addRouteDetailsDTO = new RouteDetailsDTO()
            {
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            };
            List<RouteDetailsDTO> stops = new List<RouteDetailsDTO>() { addRouteDetailsDTO };
            RouteDTO addRouteDTO = new RouteDTO()
            {
                Source = "Chennai",
                Destination = "Bangalore",
                RouteStops = stops
            };

            // Action
            var result = await routeService.AddRoute(addRouteDTO);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Source, Is.EqualTo("Chennai"));
                Assert.That(result.Destination, Is.EqualTo("Bangalore"));
            });
        }

        #endregion


        #region Get All Routes Test

        [Test]
        public async Task GetAllRouteSuccessTest()
        {
            // Arrange
            RouteDetailsDTO addRouteDetailsDTO = new RouteDetailsDTO()
            {
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            };
            List<RouteDetailsDTO> stops = new List<RouteDetailsDTO>() { addRouteDetailsDTO };
            RouteDTO addRouteDTO = new RouteDTO()
            {
                Source = "Chennai",
                Destination = "Bangalore",
                RouteStops = stops
            };
            await routeService.AddRoute(addRouteDTO);

            // Action
            var routes = await routeService.GetAllRoutes();

            // Assert
            Assert.That(routes.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllRouteExceptionTestTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await routeService.GetAllRoutes());
        }

        #endregion

    }
}
