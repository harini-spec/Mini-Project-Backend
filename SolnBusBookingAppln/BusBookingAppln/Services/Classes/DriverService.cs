using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Models.DTOs.Schedule;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BusBookingAppln.Services.Classes
{
    public class DriverService : IDriverService
    {
        private readonly IRepository<int, Driver> _driverWithSchedulesRepo;
        private readonly IRepository<int, DriverDetail> _driverDetailRepo;
        private readonly ITokenService _tokenService;


        public DriverService(IRepository<int, Driver> driverWithScheduleRepo, ITokenService tokenService, IRepository<int, DriverDetail> driverDetailRepo)
        {
            _tokenService = tokenService;
            _driverDetailRepo = driverDetailRepo;
            _driverWithSchedulesRepo = driverWithScheduleRepo;
        }


        // Get driver object by Email ID
        public async Task<Driver> GetDriverByEmail(string email)
        {
            try
            {
                var drivers = await _driverWithSchedulesRepo.GetAll();
                var driver = drivers.ToList().FirstOrDefault(x => x.Email == email);
                return driver;
            }
            catch(NoItemsFoundException)
            {
                return null;
            }
        }


        // Login driver if their account is active
        public async Task<LoginDriverOutputDTO> LoginDriver(LoginDriverInputDTO loginInputDTO)
        {
            try
            {
                // Checking if Email ID is present 
                Driver driver = await GetDriverByEmail(loginInputDTO.Email);
                if (driver == null)
                {
                    throw new UnauthorizedUserException("Invalid username or password");
                }

                DriverDetail driverDetail = await _driverDetailRepo.GetById(driver.Id);

                HMACSHA512 hMACSHA = new HMACSHA512(driverDetail.PasswordHashKey);
                var encryptedPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginInputDTO.Password));
                bool isPasswordSame = ComparePassword(encryptedPass, driverDetail.PasswordEncrypted);

                // Checking if password is correct
                if (isPasswordSame)
                {
                    // Checking if account is active
                    if(driverDetail.Status == "Active")
                    {
                        LoginDriverOutputDTO loginOutputDTO = MapDriverToLoginDriverReturnDTO(driver);
                        return loginOutputDTO;
                    }
                    throw new UserNotActiveException("Your account is not activated");
                }
                throw new UnauthorizedUserException("Invalid username or password");
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Map Driver to LoginDriverOutputDTO
        private LoginDriverOutputDTO MapDriverToLoginDriverReturnDTO(Driver driver)
        {
            LoginDriverOutputDTO loginDriverOutputDTO = new LoginDriverOutputDTO();
            loginDriverOutputDTO.DriverID = driver.Id;
            loginDriverOutputDTO.Name = driver.Name;
            loginDriverOutputDTO.Token = _tokenService.GenerateToken(driver);
            loginDriverOutputDTO.Role = "Driver";
            return loginDriverOutputDTO;
        }


        // Compare password in db to user's password - Both encrypted using same key 
        private bool ComparePassword(byte[] encryptedPass, byte[] passwordEncrypted)
        {
            for (int i = 0; i < encryptedPass.Length; i++)
            {
                if (encryptedPass[i] != passwordEncrypted[i])
                {
                    return false;
                }
            }
            return true;
        }


        // Change password of driver account
        public async Task<string> ChangePassword(string email, string NewPassword)
        {
            // Validation - password must be atleast 8 characters
            if(NewPassword.Length >= 8)
            {
                // Checking if driver account present
                Driver driver = await GetDriverByEmail(email);
                if (driver == null)
                {
                    throw new UnauthorizedUserException("Invalid username or password");
                }
                DriverDetail driverDetail = await _driverDetailRepo.GetById(driver.Id);

                HMACSHA512 hMACSHA = new HMACSHA512();
                driverDetail.PasswordHashKey = hMACSHA.Key;
                driverDetail.PasswordEncrypted = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(NewPassword));
                await _driverDetailRepo.Update(driverDetail, driverDetail.DriverId);

                return "Password successfully changed";
            }
            throw new ValidationErrorExcpetion("Password must be atleast 8 characters");
        }


        // Check if driver is booked/available during a period of time
        public async Task<bool> CheckIfDriverAvailable(AddScheduleDTO addScheduleDTO, int driverId)
        {
            Driver driver = await GetDriverById(driverId);
            List<Schedule> schedules = driver.SchedulesForDriver.ToList();
            if (schedules.Count == 0)
                return true;
            foreach (var schedule in schedules)
                {
                    if ((addScheduleDTO.DateTimeOfDeparture >= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfArrival) ||
                            (addScheduleDTO.DateTimeOfArrival >= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfArrival) ||
                            (addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfArrival >= schedule.DateTimeOfArrival))
                    {
                        return false;
                    }
                }
            return true;
        }


        public async Task<Driver> GetDriverById(int DriverId)
        {
            return await _driverWithSchedulesRepo.GetById(DriverId);
        }
    }
}
