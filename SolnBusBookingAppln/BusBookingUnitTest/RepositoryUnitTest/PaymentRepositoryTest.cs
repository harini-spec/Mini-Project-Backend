using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class PaymentRepositoryTest
    {
        BusBookingContext context;
        IRepository<string, Payment> PaymentRepository;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("PaymentRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
            PaymentRepository = new MainRepository<string, Payment>(context);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task GetAllPaymentsFailTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(() => PaymentRepository.GetAll());
        }

        [Test]
        public async Task AddPaymentSuccessTest()
        {
            // Action
            var result = await PaymentRepository.Add(new Payment
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddPaymentInvalidOperationExceptionTest()
        {
            // Arrange
            var entity = await PaymentRepository.Add(new Payment
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(() => PaymentRepository.Add(new Payment
            {
                TransactionId = entity.TransactionId,
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            }));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByTransactionIdSuccessTest()
        {
            // Arrange
            var entity = await PaymentRepository.Add(new Payment
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            });

            // Action
            var result = await PaymentRepository.GetById(entity.TransactionId);

            // Assert
            Assert.That(result.TransactionId, Is.EqualTo(entity.TransactionId));
        }

        [Test]
        public async Task GetByTransactionIdFailureTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => PaymentRepository.GetById("ABC"));
        }

        [Test]
        public async Task DeleteByTransactionIdExceptionTest()
        {
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => PaymentRepository.Delete("ABC"));
        }

        [Test]
        public async Task UpdateByTransactionIdExceptionTest()
        {
            // Arrange
            Payment payment = new Payment
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => PaymentRepository.Update(payment, "ABC"));
        }

        [Test]
        public async Task GetAllPaymentsSuccessTest()
        {
            // Arrange
            await PaymentRepository.Add(new Payment
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            });

            // Action
            var result = await PaymentRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task DeletePaymentSuccessTest()
        {
            // Arrange
            var entity = await PaymentRepository.Add(new Payment
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            });

            // Action
            var result = await PaymentRepository.Delete(entity.TransactionId);

            // Assert
            Assert.That(entity.TransactionId, Is.EqualTo(result.TransactionId));
        }

        [Test]
        public async Task UpdatePaymentSuccessTest()
        {
            // Arrange
            var InsertedEntity = await PaymentRepository.Add(new Payment
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                PaymentMethod = "GPay",
                AmountPaid = 500,
                Status = "Success",
                TicketId = 1
            });
            var entity = await PaymentRepository.GetById(InsertedEntity.TransactionId);
            entity.Status = "Fail";

            // Action
            var result = await PaymentRepository.Update(entity, entity.TransactionId);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Fail"));
        }
    }
}
