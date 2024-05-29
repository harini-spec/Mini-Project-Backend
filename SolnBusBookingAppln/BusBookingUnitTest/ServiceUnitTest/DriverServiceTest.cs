using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
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
    public class DriverServiceTest
    {
        IRepository<int, Driver> _driverWithSchedulesRepo;
        IRepository<int, DriverDetail> _driverDetailRepo;
        ITokenService _tokenService;

        BusBookingContext context;

        IDriverService DriverService;
        IAdminService AdminService;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("RouteRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);

            _driverDetailRepo = new MainRepository<int, DriverDetail>(context);
            _driverWithSchedulesRepo = new DriverWithScheduleRepository(context);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the Secret Key to generate JWT Token for SHA256");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            _tokenService = new TokenService(mockConfig.Object);

            DriverService = new DriverService(_driverWithSchedulesRepo, _tokenService, _driverDetailRepo);
            AdminService = new AdminService(_driverWithSchedulesRepo, _driverDetailRepo);
        }


        [Test, Order(1)]
        public async Task LoginDriverAccountNotActiveFailTest()
        {
            // Arrange
            RegisterDriverInputDTO driver = new RegisterDriverInputDTO()
            {
                Name = "Sarah",
                Age = 30,
                Email = "sarah@gmail.com",
                Phone = "8877887788",
                YearsOfExperience = 4,
                Password = "sarahroot"
            };
            await AdminService.RegisterDriver(driver);
            LoginDriverInputDTO loginDriverInputDTO = new LoginDriverInputDTO()
            {
                Email = "sarah@gmail.com",
                Password = "sarahroot"
            };

            // Action
            var exception = Assert.ThrowsAsync<UserNotActiveException>(async () => await DriverService.LoginDriver(loginDriverInputDTO));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Your account is not activated"));
        }

        [Test, Order(2)]
        public async Task LoginDriverSuccessTest()
        {
            // Arrange
            await AdminService.ActivateDriver(1);
            LoginDriverInputDTO loginDriverInputDTO = new LoginDriverInputDTO()
            {
                Email = "sarah@gmail.com",
                Password = "sarahroot"
            };

            // Action
            var result = await DriverService.LoginDriver(loginDriverInputDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(3)]
        public async Task LoginDriverWrongPasswordFailTest()
        {
            // Arrange
            LoginDriverInputDTO loginDriverInputDTO = new LoginDriverInputDTO()
            {
                Email = "sarah@gmail.com",
                Password = "sararoot"
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await DriverService.LoginDriver(loginDriverInputDTO));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid username or password"));
        }

        [Test, Order(3)]
        public async Task LoginDriverEmailDoesNotExistsFailTest()
        {
            // Arrange
            LoginDriverInputDTO loginDriverInputDTO = new LoginDriverInputDTO()
            {
                Email = "sara@gmail.com",
                Password = "sarahroot"
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await DriverService.LoginDriver(loginDriverInputDTO));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid username or password"));
        }

        [Test, Order(4)]
        public async Task ChangePasswordValidationExceptionTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<ValidationErrorExcpetion>(async () => await DriverService.ChangePassword("sarah@gmail.com", "sarroot"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Password must be atleast 8 characters"));
        }

        [Test, Order(5)]
        public async Task ChangePasswordEmailDoesNotExistExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () => await DriverService.ChangePassword("abc@gmail.com", "sarahroot"));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid username or password"));
        }

        [Test, Order(6)]
        public async Task ChangePasswordSuccessTest()
        {
            // Action
            var result = await DriverService.ChangePassword("sarah@gmail.com", "sararoot");

            // Assert
            Assert.That(result, Is.EqualTo("Password successfully changed"));
        }
    }
}
