using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class PaymentRepositoryTest
    {
        BusBookingContext context;
        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("PaymentRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
        }


        [Test]
        public async Task AddPaymentSuccessTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);

            // Action
            var result = await paymentRepository.Add(new Payment
            {
                TransactionId = "ABC123",
                TotalAmount = 5000,
                PaymentMethod = "Paytm",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Assert
            Assert.That(result.TransactionId, Is.EqualTo("ABC123"));

            await paymentRepository.Delete("ABC123");
        }

        [Test]
        public async Task AddPaymentInvalidOperationExceptionTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);
            await paymentRepository.Add(new Payment
            {
                TransactionId = "ABC456",
                TotalAmount = 5000,
                PaymentMethod = "Paytm",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await paymentRepository.Add(new Payment
            {
                TransactionId = "ABC456",
                TotalAmount = 5000,
                PaymentMethod = "Paytm",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetPaymentByTransactionIdSuccessTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);
            await paymentRepository.Add(new Payment
            {
                TransactionId = "ABC789",
                TotalAmount = 5000,
                PaymentMethod = "Paytm",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var result = await paymentRepository.GetById("ABC789");

            // Assert
            Assert.That(result.TransactionId, Is.EqualTo("ABC789"));

            await paymentRepository.Delete("ABC789");
        }

        [Test]
        public async Task GetPaymentByTransactionIdFailureTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await paymentRepository.GetById("ABC100"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Payment with Transaction ID = ABC100 not found."));
        }

        [Test]
        public async Task DeletePaymentByTransactionIdExceptionTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await paymentRepository.Delete("ABC100"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Payment with Transaction ID = ABC100 not found."));
        }

        [Test]
        public async Task GetAllPaymentsFailTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);
            // Used here becuz if used in exception, add in exception is not async so order is changing 
            await paymentRepository.Delete("ABC456");

            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await paymentRepository.GetAll());

            // Assert
            Assert.That(exception.Message, Is.EqualTo("No entities of type Payment are found."));
        }

        [Test]
        public async Task GetAllPaymentsSuccessTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);
            await paymentRepository.Add(new Payment
            {
                TransactionId = "ABC222",
                TotalAmount = 5000,
                PaymentMethod = "Paytm",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var result = await paymentRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));

            await paymentRepository.Delete("ABC222");
        }

        [Test]
        public async Task UpdatePaymentExceptionTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);
            var payment = new Payment
            {
                TransactionId = "ABC100",
                TotalAmount = 5000,
                PaymentMethod = "GPay",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await paymentRepository.Update(payment, "ABC100"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type Payment with Transaction ID = ABC100 not found."));
        }

        [Test]
        public async Task DeletePaymentSuccessTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);
            await paymentRepository.Add(new Payment
            {
                TransactionId = "ABC333",
                TotalAmount = 5000,
                PaymentMethod = "Paytm",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });

            // Action
            var entity = await paymentRepository.Delete("ABC333");

            // Assert
            Assert.That(entity.TransactionId, Is.EqualTo("ABC333"));
        }

        [Test]
        public async Task UpdatePaymentSuccessTest()
        {
            // Arrange
            IRepository<string, Payment> paymentRepository = new PaymentRepository(context);
            await paymentRepository.Add(new Payment
            {
                TransactionId = "ABC444",
                TotalAmount = 5000,
                PaymentMethod = "Paytm",
                PaymentDate = DateTime.Now,
                Status = "SUCCESS",
                TicketId = 1
            });
            var entity = await paymentRepository.GetById("ABC444");
            entity.PaymentMethod = "GPay";

            // Action
            var result = await paymentRepository.Update(entity, "ABC444");

            // Assert
            Assert.That(result.PaymentMethod, Is.EqualTo("GPay"));
        }
    }
}
