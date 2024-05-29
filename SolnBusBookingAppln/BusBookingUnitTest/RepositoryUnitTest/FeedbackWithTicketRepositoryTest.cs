using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class FeedbackWithTicketRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("FeedbackRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }


        [Test]
        public async Task AddFeedbackSuccessTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);

            Ticket ticket = new Ticket(){Id = 1};

            // Action
            var result = await feedbackRepository.Add(new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 1,
                FeedbackForTicket = ticket
            });
            var entity = await feedbackRepository.GetAll();
            // Assert
            Assert.That(result.TicketId, Is.EqualTo(1));
            await feedbackRepository.Delete(1);
        }

        [Test]
        public async Task AddFeedbackInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);
            Ticket ticket = new Ticket() { Id = 2 };
            await feedbackRepository.Add(new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 2,
                FeedbackForTicket = ticket
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => feedbackRepository.Add(new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 2,
                FeedbackForTicket = ticket
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByIdSuccessTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);
            Ticket ticket = new Ticket() { Id = 3 };
            await feedbackRepository.Add(new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 3,
                FeedbackForTicket = ticket
            });

            // Action
            var result = await feedbackRepository.GetById(3);

            // Assert
            Assert.That(result.TicketId, Is.EqualTo(3));

            await feedbackRepository.Delete(3);
        }

        [Test]
        public async Task GetByIdFailureTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => feedbackRepository.GetById(100));

        }

        [Test]
        public async Task DeleteByIdExceptionTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => feedbackRepository.Delete(100));
        }

        [Test]
        public async Task UpdateByIdExceptionTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);
            Ticket ticket = new Ticket() { Id = 3 };
            Feedback feedback = new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 3,
                FeedbackForTicket = ticket
            };

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => feedbackRepository.Update(feedback, 100));
        }

        [Test]
        public async Task GetAllFeedbacksFailTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);
            await feedbackRepository.Delete(2);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => feedbackRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Feedback are found."));
        }

        [Test]
        public async Task GetAllFeedbacksSuccessTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);
            Ticket ticket = new Ticket() { Id = 4 };
            await feedbackRepository.Add(new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 4,
                FeedbackForTicket = ticket
            });

            // Action
            var result = await feedbackRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            await feedbackRepository.Delete(4);
        }

        [Test]
        public async Task DeleteFeedbackSuccessTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);
            Ticket ticket = new Ticket() { Id = 5 };
            await feedbackRepository.Add(new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 5,
                FeedbackForTicket = ticket
            });

            // Action
            var entity = await feedbackRepository.Delete(5);

            // Assert
            Assert.That(entity.TicketId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateFeedbackSuccessTest()
        {
            // Arrange
            IRepository<int, Feedback> feedbackRepository = new FeedbackWithTicketRepository(context);
            Ticket ticket = new Ticket() { Id = 6 };
            await feedbackRepository.Add(new Feedback
            {
                FeedbackDate = DateTime.Now,
                Message = "Timely arrival",
                Rating = 9,
                TicketId = 6,
                FeedbackForTicket = ticket
            });
            var entity = await feedbackRepository.GetById(6);
            entity.Message = "Good work";

            // Action
            var result = await feedbackRepository.Update(entity, 6);

            // Assert
            Assert.That(result.Message, Is.EqualTo("Good work"));
            await feedbackRepository.Delete(6);
        }
    }
}

