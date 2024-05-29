using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EmployeeRequestTrackerApplnTest
{
    public class TokenServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateTokenForDriverPassTest()
        {
            //Arrange
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the Secret Key to generate JWT Token for SHA256");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenService service = new TokenService(mockConfig.Object);

            //Action
            var token = service.GenerateToken(new Driver {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                YearsOfExperience = 2
            });

            //Assert
            Assert.IsNotNull(token);
        }

        [Test]
        public void CreateTokenForCustomerPassTest()
        {
            //Arrange
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the Secret Key to generate JWT Token for SHA256");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenService service = new TokenService(mockConfig.Object);

            //Action
            var token = service.GenerateToken(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            });

            //Assert
            Assert.IsNotNull(token);
        }

        [Test]
        public void CreateTokenForAdminPassTest()
        {
            //Arrange
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the Secret Key to generate JWT Token for SHA256");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenService service = new TokenService(mockConfig.Object);

            //Action
            var token = service.GenerateToken(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Admin"
            });

            //Assert
            Assert.IsNotNull(token);
        }
    }
}