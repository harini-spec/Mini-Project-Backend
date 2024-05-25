using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BusBookingAppln.Services.Classes
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<int, Driver> _driverRepo;
        private readonly IRepository<int, DriverDetail> _driverDetailRepo;
        private readonly ITokenService _tokenService;
        private readonly IDriverService _driverService;

        public AdminService(IDriverService driverService, ITokenService tokenService, IRepository<int, Driver> driverRepo, IRepository<int, DriverDetail> driverDetailRepo)
        {
            _tokenService = tokenService;
            _driverRepo = driverRepo;
            _driverDetailRepo = driverDetailRepo;
            _driverService = driverService;
        }

        public async Task<RegisterDriverOutputDTO> RegisterDriver(RegisterDriverInputDTO registerInputDTO)
        {
            Driver driver = null;
            DriverDetail driverDetail = null;
            DriverDetail InsertedDriverDetail = null;

            try
            {
                Driver ExistingDriver = await _driverService.GetDriverByEmail(registerInputDTO.Email);
                if (ExistingDriver != null) { throw new UnableToRegisterException("Email ID already exists"); }
                driver = MapRegisterDriverInputDTOToDriver(registerInputDTO);
                driver = await _driverRepo.Add(driver);

                driverDetail = MapRegisterInputDTOToDriverDetail(registerInputDTO);
                driverDetail.DriverId = driver.Id;
                InsertedDriverDetail = await _driverDetailRepo.Add(driverDetail);
                RegisterDriverOutputDTO registerDriverOutputDTO = MapDriverToRegisterDriverOutputDTO(driver);
                return registerDriverOutputDTO;
            }
            catch (UnableToRegisterException) { throw; }
            catch (Exception) { }
            if (driver == null)
            {
                throw new UnableToRegisterException("Not able to register at this moment");
            }
            if (InsertedDriverDetail == null)
            {
                await RevertDriverInsert(driver);
            }
            throw new UnableToRegisterException("Not able to register at this moment");
        }

        private async Task RevertDriverInsert(Driver driver)
        {
            await _driverRepo.Delete(driver.Id);
        }

        private RegisterDriverOutputDTO MapDriverToRegisterDriverOutputDTO(Driver driver)
        {
            RegisterDriverOutputDTO registerDriverOutputDTO = new RegisterDriverOutputDTO();
            registerDriverOutputDTO.Id = driver.Id;
            registerDriverOutputDTO.Name = driver.Name;
            registerDriverOutputDTO.Age = driver.Age;
            registerDriverOutputDTO.Email = driver.Email;
            registerDriverOutputDTO.Phone = driver.Phone;
            registerDriverOutputDTO.YearsOfExperience = driver.YearsOfExperience;
            return registerDriverOutputDTO;
        }

        private DriverDetail MapRegisterInputDTOToDriverDetail(RegisterDriverInputDTO registerInputDTO)
        {
            DriverDetail driverDetail = new DriverDetail();
            driverDetail.Status = "Inactive";

            HMACSHA512 hMACSHA = new HMACSHA512();
            driverDetail.PasswordHashKey = hMACSHA.Key;
            driverDetail.PasswordEncrypted = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(registerInputDTO.Password));
            return driverDetail;
        }

        private Driver MapRegisterDriverInputDTOToDriver(RegisterDriverInputDTO registerInputDTO)
        {
            Driver driver = new Driver();
            driver.Name = registerInputDTO.Name;
            driver.Age = registerInputDTO.Age;
            driver.Email = registerInputDTO.Email;
            driver.Phone = registerInputDTO.Phone;
            driver.YearsOfExperience = registerInputDTO.YearsOfExperience;
            return driver;
        }

        public async Task<DriverActivateReturnDTO> ActivateDriver(int DriverId)
        {
            DriverDetail driverDetail = await _driverDetailRepo.GetById(DriverId);
            driverDetail.Status = "Active";
            driverDetail = await _driverDetailRepo.Update(driverDetail, DriverId);
            Driver driver = await _driverRepo.GetById(DriverId);
            DriverActivateReturnDTO driverActivateReturnDTO = MapDriverToDriverActivateReturnDTO(driver, "Active");
            return driverActivateReturnDTO;
        }

        private DriverActivateReturnDTO MapDriverToDriverActivateReturnDTO(Driver driver, string status)
        {
            DriverActivateReturnDTO driverActivateReturnDTO = new DriverActivateReturnDTO();
            driverActivateReturnDTO.DriverId = driver.Id;
            driverActivateReturnDTO.Email = driver.Email;
            driverActivateReturnDTO.Status = status;
            return driverActivateReturnDTO;
        }
    }
}
