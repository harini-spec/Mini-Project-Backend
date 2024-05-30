using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class RefundRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("RefundRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }

        [Test, Order(1)]
        public async Task GetAllRefundsFailTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => RefundRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Refund are found."));
        }

        [Test, Order(2)]
        public async Task AddRefundSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);

            // Action
            var result = await RefundRepository.Add(new Refund
            {
                TransactionId = Guid.NewGuid().ToString(),
                RefundAmount = 50,
                RefundDate = DateTime.Now,
                Status = "Success",
                TicketId = 1
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(3)]
        public async Task AddRefundInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);
            var result = await RefundRepository.Add(new Refund
            {
                TransactionId = Guid.NewGuid().ToString(),
                RefundAmount = 50,
                RefundDate = DateTime.Now,
                Status = "Success",
                TicketId = 1
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => RefundRepository.Add(new Refund
            {
                TransactionId = result.TransactionId,
                RefundAmount = 50,
                RefundDate = DateTime.Now,
                Status = "Success",
                TicketId = 1
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test, Order(4)]
        public async Task GetByTransactionIdSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);
            var entity = await RefundRepository.Add(new Refund
            {
                TransactionId = Guid.NewGuid().ToString(),
                RefundAmount = 50,
                RefundDate = DateTime.Now,
                Status = "Success",
                TicketId = 1
            });

            // Action
            var result = await RefundRepository.GetById(entity.TransactionId);

            // Assert
            Assert.That(result.TransactionId, Is.EqualTo(entity.TransactionId));
        }

        [Test, Order(5)]
        public async Task GetByTransactionIdFailureTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RefundRepository.GetById("ABC"));
        }

        [Test, Order(6)]
        public async Task DeleteByTransactionIdExceptionTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RefundRepository.Delete("ABC"));
        }

        [Test, Order(7)]
        public async Task UpdateByTransactionIdExceptionTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);
            Refund refund = new Refund
            {
                TransactionId = Guid.NewGuid().ToString(),
                RefundAmount = 50,
                RefundDate = DateTime.Now,
                Status = "Success",
                TicketId = 1
            };

           var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => RefundRepository.Update(refund, refund.TransactionId));

        }

        [Test, Order(8)]
        public async Task GetAllRefundSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);

            // Action
            var result = await RefundRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test, Order(9)]
        public async Task DeleteRefundSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);
            var entity = await RefundRepository.Add(new Refund
            {
                TransactionId = Guid.NewGuid().ToString(),
                RefundAmount = 50,
                RefundDate = DateTime.Now,
                Status = "Success",
                TicketId = 1
            });

            // Action
            var result = await RefundRepository.Delete(entity.TransactionId);

            // Assert
            Assert.That(result.TransactionId, Is.EqualTo(entity.TransactionId));
        }

        [Test]
        public async Task UpdateRefundSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> RefundRepository = new MainRepository<string, Refund>(context);
            var InsertedEntity = await RefundRepository.Add(new Refund
            {
                TransactionId = Guid.NewGuid().ToString(),
                RefundAmount = 50,
                RefundDate = DateTime.Now,
                Status = "Success",
                TicketId = 1
            });
            var entity = await RefundRepository.GetById(InsertedEntity.TransactionId);
            entity.Status = "Fail";

            // Action
            var result = await RefundRepository.Update(entity, InsertedEntity.TransactionId);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Fail"));
        }
    }
}
