﻿using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingUnitTest.ServiceUnitTest
{
    public class UserAndCustomerServiceTest
    {
        IRepository<int, User> UserRepo;
        IRepository<int, UserDetail> UserDetailRepo;
        IRepository<int, Ticket> TicketRepository;

        ITicketService ticketService;
        ITokenService tokenService;
        IUserService userService;
        ICustomerService customerService;
        ISeatAvailability seatAvailabilityService;

        BusBookingContext context;

        Mock<ILogger<CustomerAccountService>> CustomerAccountLogger;
        Mock<ILogger<UserService>> UserServiceLogger;
        Mock<ILogger<TicketService>> TicketLogger;
        Mock<ILogger<SeatAvailabilityService>> SeatAvailabilityServiceLogger;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("UserAndCustomerDB");
            context = new BusBookingContext(optionsBuilder.Options);

            UserRepo = new MainRepository<int, User>(context);
            UserDetailRepo = new MainRepository<int, UserDetail>(context);
            TicketRepository = new TicketWithTicketDetailsRepository(context);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the Secret Key to generate JWT Token for SHA256");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            tokenService = new TokenService(mockConfig.Object);

            CustomerAccountLogger = new Mock<ILogger<CustomerAccountService>>();
            UserServiceLogger = new Mock<ILogger<UserService>>();
            SeatAvailabilityServiceLogger = new Mock<ILogger<SeatAvailabilityService>>();
            TicketLogger = new Mock<ILogger<TicketService>>();

            userService = new UserService(UserRepo, UserDetailRepo, tokenService, UserServiceLogger.Object);
            seatAvailabilityService = new SeatAvailabilityService(null, null, TicketRepository, null, SeatAvailabilityServiceLogger.Object);
            ticketService = new TicketService(null, null, seatAvailabilityService, TicketRepository, null, null, TicketLogger.Object);
            customerService = new CustomerAccountService(ticketService, UserDetailRepo, userService, CustomerAccountLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        #region Login User Tests

        [Test]
        public async Task LoginUserSuccessTest()
        {
            // Arrange
            RegisterInputDTO Admin = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Admin, "Admin");
            LoginInputDTO adminLogin = new LoginInputDTO()
            {
                Email = "sam@gmail.com",
                Password = "samaroot"
            };

            // Action
            var result = await userService.LoginAdminAndCustomer(adminLogin, "Admin");

            // Assert
            Assert.That(result.Role, Is.EqualTo("Admin"));
        }

        [Test]
        public async Task LoginUserIncorrectEmailExceptionTest()
        {
            // Arrange

            LoginInputDTO adminLogin = new LoginInputDTO()
            {
                Email = "dana@gmail.com",
                Password = "sarahroot"
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await userService.LoginAdminAndCustomer(adminLogin, "Customer"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid username or password"));
        }

        [Test]
        public async Task LoginUserIncorrectPasswordExceptionTest()
        {
            // Arrange

            LoginInputDTO adminLogin = new LoginInputDTO()
            {
                Email = "sarah@gmail.com",
                Password = "danahroot"
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await userService.LoginAdminAndCustomer(adminLogin, "Customer"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid username or password"));
        }

        [Test]
        public async Task LoginUserNotActiveExceptionTest()
        {
            // Arrange
            RegisterInputDTO Customer = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Customer, "Customer");
            LoginInputDTO customerLogin = new LoginInputDTO()
            {
                Email = "sam@gmail.com",
                Password = "samaroot"
            };
            await customerService.SoftDeleteCustomerAccount(1);

            // Action
            var exception = Assert.ThrowsAsync<UserNotActiveException>(async () => await userService.LoginAdminAndCustomer(customerLogin, "Customer"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Your account is not activated"));
        }

        #endregion

        #region Activate Customer Account Tests

        [Test]
        public async Task ActivateUserOnlyCustomerCanActiveExceptionTest()
        {
            // Arrange

            RegisterInputDTO Admin = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Admin, "Admin");
            LoginInputDTO adminLogin = new LoginInputDTO()
            {
                Email = "sam@gmail.com",
                Password = "samaroot"
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await customerService.ActivateDeletedCustomerAccount(adminLogin));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Only Customer account can be activated here"));
        }

        [Test]
        public async Task ActivateUserAlreadyActiveExceptionTest()
        {
            // Arrange
            RegisterInputDTO Customer = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Customer, "Customer");
            LoginInputDTO CustomerLogin = new LoginInputDTO()
            {
                Email = "sam@gmail.com",
                Password = "samaroot"
            };

            // Action
            var exception = Assert.ThrowsAsync<IncorrectOperationException>(async () => await customerService.ActivateDeletedCustomerAccount(CustomerLogin));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("User account is already active"));
        }

        [Test]
        public async Task ActivateUserSuccessTest()
        {
            // Arrange
            RegisterInputDTO Customer = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "samu@gmail.com",
                Phone = "8877887788",
                Password = "samuroot"
            };
            await userService.RegisterAdminAndCustomer(Customer, "Customer");
            await customerService.SoftDeleteCustomerAccount(1);
            LoginInputDTO CustomerLogin = new LoginInputDTO()
            {
                Email = "samu@gmail.com",
                Password = "samuroot"
            };

            // Action
            var result = await customerService.ActivateDeletedCustomerAccount(CustomerLogin);

            // Assert
            Assert.That(result.Role, Is.EqualTo("Customer"));
        }

        #endregion

        #region Soft Delete Customer Account Tests

        [Test]
        public async Task SoftDeleteCustomerAccountActiveTicketsFailTest()
        {
            // Arrange
            RegisterInputDTO Customer = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Customer, "Customer");
            await TicketRepository.Add(new Ticket()
            {
                Id = 100,
                Status = "Booked",
                ScheduleId = 1,
                DiscountPercentage = 0,
                Total_Cost = 50,
                Final_Amount = 50,
                UserId = 1,
                DateAndTimeOfAdding = DateTime.Now
            });

            // Action
            var result = await customerService.SoftDeleteCustomerAccount(1);

            // Assert
            Assert.That(result, Is.EqualTo("Sorry, you have active tickets. Cannot delete your account now"));
        }

        [Test]
        public async Task SoftDeleteCustomerAccountAlreadyDeletedExceptionTest()
        {
            // Arrange
            RegisterInputDTO Customer = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Customer, "Customer");
            var result = await customerService.SoftDeleteCustomerAccount(1);

            // Action
            var exception = Assert.ThrowsAsync<UserNotActiveException>(async () => await customerService.SoftDeleteCustomerAccount(1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("User account is already deleted"));
        }

        [Test]
        public async Task SoftDeleteCustomerAccountSuccessTest()
        {
            // Arrange
            RegisterInputDTO Customer = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await TicketRepository.Add(new Ticket()
            {
                Id = 100,
                Status = "Not Booked",
                ScheduleId = 1,
                DiscountPercentage = 0,
                Total_Cost = 50,
                Final_Amount = 50,
                UserId = 1,
                DateAndTimeOfAdding = DateTime.Now
            });
            await userService.RegisterAdminAndCustomer(Customer, "Customer");
            LoginInputDTO CustomerLogin = new LoginInputDTO()
            {
                Email = "sam@gmail.com",
                Password = "samaroot"
            };

            // Action
            var result = await customerService.SoftDeleteCustomerAccount(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task SoftDeleteCustomerAccountWithNoCustomerTicketsSuccessTest()
        {
            // Arrange
            RegisterInputDTO Customer = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Customer, "Customer");
            await TicketRepository.Add(new Ticket()
            {
                Id = 100,
                Status = "Not Booked",
                ScheduleId = 1,
                DiscountPercentage = 0,
                Total_Cost = 50,
                Final_Amount = 50,
                UserId = 1,
                DateAndTimeOfAdding = DateTime.Now
            });
            LoginInputDTO CustomerLogin = new LoginInputDTO()
            {
                Email = "sam@gmail.com",
                Password = "samaroot"
            };

            // Action
            var result = await customerService.SoftDeleteCustomerAccount(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        #endregion

        #region Register User Tests

        [Test]
        public async Task RegisterUserSuccessTest()
        {
            // Arrange

            RegisterInputDTO admin = new RegisterInputDTO()
            {
                Name = "Sarah",
                Age = 30,
                Email = "sarah@gmail.com",
                Phone = "8877887788",
                Password = "sarahroot",
            };

            // Action
            var result = await userService.RegisterAdminAndCustomer(admin, "Admin");

            // Assert
            Assert.That(result.Email, Is.EqualTo("sarah@gmail.com"));
        }

        [Test]
        public async Task RegisterUserFailTest()
        {
            // Arrange
            RegisterInputDTO admin = new RegisterInputDTO()
            {
                Name = "Sarah",
                Age = 30,
                Phone = "8877887788",
                Password = "sarahroot",
            };

            // Action
            var exception = Assert.ThrowsAsync<UnableToRegisterException>(async () => await userService.RegisterAdminAndCustomer(admin, "Admin"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Not able to register at this moment"));
        }

        [Test]
        public async Task RegisterUserEmailAlreadyExistsExceptionTest()
        {
            // Arrange
            RegisterInputDTO Admin = new RegisterInputDTO()
            {
                Name = "Sam",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "samaroot"
            };
            await userService.RegisterAdminAndCustomer(Admin, "Admin");
            RegisterInputDTO admin = new RegisterInputDTO()
            {
                Name = "Sarah",
                Age = 30,
                Email = "sam@gmail.com",
                Phone = "8877887788",
                Password = "sarahroot",
            };

            // Action
            var exception = Assert.ThrowsAsync<UnableToRegisterException>(async () => await userService.RegisterAdminAndCustomer(admin, "Admin"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Email ID already exists"));
        }

        #endregion
    }
}
