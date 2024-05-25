using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

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


        [Test]
        public async Task AddRefundSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);

            // Action
            var result = await refundRepository.Add(new Refund
            {
                TransactionId = "ABC123",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Assert
            Assert.That(result.TransactionId, Is.EqualTo("ABC123"));

            await refundRepository.Delete("ABC123");
        }

        [Test]
        public async Task AddRefundInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);
            var result = await refundRepository.Add(new Refund
            {
                TransactionId = "ABC456",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await refundRepository.Add(new Refund
            {
                TransactionId = "ABC456",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetRefundByTransactionIdSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);
            await refundRepository.Add(new Refund
            {
                TransactionId = "ABC789",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var result = await refundRepository.GetById("ABC789");

            // Assert
            Assert.That(result.TransactionId, Is.EqualTo("ABC789"));

            await refundRepository.Delete("ABC789");
        }

        [Test]
        public async Task GetRefundByTransactionIdFailureTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await refundRepository.GetById("ABC100"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Refund with Transaction ID = ABC100 not found."));
        }

        [Test]
        public async Task DeleteRefundByTransactionIdExceptionTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await refundRepository.Delete("ABC100"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Refund with Transaction ID = ABC100 not found."));
        }

        [Test]
        public async Task GetAllRefundsFailTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);
            // Used here becuz if used in exception, add in exception is not async so order is changing 
            await refundRepository.Delete("ABC456");

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await refundRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Refund are found."));
        }

        [Test]
        public async Task GetAllRefundsSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);
            await refundRepository.Add(new Refund
            {
                TransactionId = "ABC222",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var result = await refundRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));

            await refundRepository.Delete("ABC222");
        }

        [Test]
        public async Task UpdateRefundExceptionTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);
            var refund = new Refund
            {
                TransactionId = "ABC100",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await refundRepository.Update(refund, "ABC100"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Refund with Transaction ID = ABC100 not found."));
        }

        [Test]
        public async Task DeleteRefundSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);
            await refundRepository.Add(new Refund
            {
                TransactionId = "ABC333",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var entity = await refundRepository.Delete("ABC333");

            // Assert
            Assert.That(entity.TransactionId, Is.EqualTo("ABC333"));
        }

        [Test]
        public async Task UpdateRefundSuccessTest()
        {
            // Arrange
            IRepository<string, Refund> refundRepository = new RefundRepository(context);
            await refundRepository.Add(new Refund
            {
                TransactionId = "ABC444",
                RefundAmount = 5000,
                RefundDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });
            var entity = await refundRepository.GetById("ABC444");
            entity.RefundAmount = 2000;

            // Action
            var result = await refundRepository.Update(entity, "ABC444");

            // Assert
            Assert.That(result.RefundAmount, Is.EqualTo(2000));
        }
    }
}
