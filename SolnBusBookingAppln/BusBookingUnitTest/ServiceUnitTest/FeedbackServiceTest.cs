using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Feedback;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingUnitTest.ServiceUnitTest
{
    public class FeedbackServiceTest
    {
        IRepository<int, Feedback> FeedbackRepo;
        IRepository<int, Ticket> TicketRepository;
        BusBookingContext context;
        IFeedbackService feedbackService;
        ITicketService ticketService;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("FeedbackDB");
            context = new BusBookingContext(optionsBuilder.Options);

            FeedbackRepo = new FeedbackWithTicketRepository(context);

            TicketRepository = new TicketWithTicketDetailsRepository(context);

            ticketService = new TicketService(null, null, null, TicketRepository, null, null);
            feedbackService = new FeedbackService(FeedbackRepo, ticketService);
        }

        #region Add Feedback Tests

        [Test, Order(1)]
        public async Task AddFeedbackSuccessTest()
        {
            // Arrange
            await TicketRepository.Add(new Ticket()
            {
                Id = 200,
                Status = "Ride Over",
                ScheduleId = 1,
                DiscountPercentage = 0,
                Total_Cost = 50,
                Final_Amount = 50,
                UserId = 100,
                DateAndTimeOfAdding = DateTime.Now
            });
            AddFeedbackDTO feedback = new AddFeedbackDTO()
            {
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 200
            };

            // Action
            var result = await feedbackService.AddFeedback(100, feedback);

            // Assert
            Assert.That(result, Is.EqualTo("Feedback successfully added"));
        }

        [Test, Order(2)]
        public async Task AddFeedbackFailureTest()
        {
            // Arrange
            await TicketRepository.Add(new Ticket()
            {
                Id = 201,
                Status = "Booked",
                ScheduleId = 1,
                DiscountPercentage = 0,
                Total_Cost = 50,
                Final_Amount = 50,
                UserId = 100,
                DateAndTimeOfAdding = DateTime.Now
            });
            AddFeedbackDTO feedback = new AddFeedbackDTO()
            {
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 201
            };

            // Action
            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await feedbackService.AddFeedback(100, feedback));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You cannot add feedback to this ticket"));
        }

        [Test, Order(3)]
        public async Task AddFeedbackWrongCustomerExceptionTest()
        {
            // Arrange
            await TicketRepository.Add(new Ticket()
            {
                Id = 202,
                Status = "Ride Over",
                ScheduleId = 1,
                DiscountPercentage = 0,
                Total_Cost = 50,
                Final_Amount = 50,
                UserId = 100,
                DateAndTimeOfAdding = DateTime.Now
            });
            AddFeedbackDTO feedback = new AddFeedbackDTO()
            {
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 202
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await feedbackService.AddFeedback(200, feedback));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("You can't provide feedback for this ticket"));
        }

        #endregion

        #region Get Feedbacks Tests

        [Test, Order(4)]
        public async Task GetAllFeedbacksSuccessTest()
        {
            // Action
            var result = await feedbackService.GetAllFeedbacksForARide(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test, Order(5)]
        public async Task GetAllFeedbacksNoFeedbackFoundExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await feedbackService.GetAllFeedbacksForARide(100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No feedbacks found"));
        }

        #endregion
    }
}
