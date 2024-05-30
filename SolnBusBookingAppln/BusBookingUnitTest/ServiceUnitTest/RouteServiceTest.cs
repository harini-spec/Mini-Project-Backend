using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs.Route;
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
    public class RouteServiceTest
    {
        IRepository<int, Route> RouteRepository;
        BusBookingContext context;
        IRouteService routeService;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("RouteDB");
            context = new BusBookingContext(optionsBuilder.Options);
            RouteRepository = new MainRepository<int, Route>(context);
            routeService = new RouteService(RouteRepository);
        }


        #region Get Route Tests

        [Test, Order(1)]
        public async Task GetRouteBySourceAndDestExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await routeService.GetRoute("Chennai", "Vellore"));
        }

        [Test, Order(2)]
        public async Task GetRouteBySourceAndDestFailTest()
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

        [Test, Order(4)]
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
            AddRouteDetailsDTO addRouteDetailsDTO = new AddRouteDetailsDTO()
            {
                StopNumber = 1,
                From_Location = "Chennai",
                To_Location = "Bangalore"
            };
            List<AddRouteDetailsDTO> stops = new List<AddRouteDetailsDTO>() { addRouteDetailsDTO };
            AddRouteDTO addRouteDTO = new AddRouteDTO()
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
    }
}
