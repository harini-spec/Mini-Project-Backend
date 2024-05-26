﻿using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
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

        public async Task<LoginDriverOutputDTO> LoginDriver(LoginDriverInputDTO loginInputDTO)
        {
            try
            {
                Driver driver = await GetDriverByEmail(loginInputDTO.Email);
                if (driver == null)
                {
                    throw new UnauthorizedUserException("Invalid username or password");
                }

                DriverDetail driverDetail = await _driverDetailRepo.GetById(driver.Id);

                HMACSHA512 hMACSHA = new HMACSHA512(driverDetail.PasswordHashKey);
                var encryptedPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginInputDTO.Password));
                bool isPasswordSame = ComparePassword(encryptedPass, driverDetail.PasswordEncrypted);
                if (isPasswordSame)
                {
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

        private LoginDriverOutputDTO MapDriverToLoginDriverReturnDTO(Driver driver)
        {
            LoginDriverOutputDTO loginDriverOutputDTO = new LoginDriverOutputDTO();
            loginDriverOutputDTO.DriverID = driver.Id;
            loginDriverOutputDTO.Name = driver.Name;
            loginDriverOutputDTO.Token = _tokenService.GenerateToken(driver);
            loginDriverOutputDTO.Role = "Driver";
            return loginDriverOutputDTO;
        }

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

        public async Task<Driver> GetDriverByEmail(string email)
        {
            var drivers = await _driverWithSchedulesRepo.GetAll();
            var driver = drivers.ToList().FirstOrDefault(x => x.Email == email);
            return driver;
        }

        public async Task<string> ChangePassword(string email, string NewPassword)
        {
            Driver driver = await GetDriverByEmail(email);
            DriverDetail driverDetail = await _driverDetailRepo.GetById(driver.Id);

            HMACSHA512 hMACSHA = new HMACSHA512();
            driverDetail.PasswordHashKey = hMACSHA.Key;
            driverDetail.PasswordEncrypted = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(NewPassword));
            await _driverDetailRepo.Update(driverDetail, driverDetail.DriverId);

            return "Password successfully changed";
        }

        public async Task<List<GetScheduleDTO>> GetAllSchedulesOfDriver(int DriverId)
        {
            Driver driver = await _driverWithSchedulesRepo.GetById(DriverId);
            List<Schedule> schedules = driver.SchedulesForDriver.ToList();
            List<GetScheduleDTO> SchedulesOfDriver = MapScheduleToGetScheduleDTO(schedules);    
            if(schedules.Count == 0)
            {
                throw new NoItemsFoundException($"No Schedules are found for Driver with Id {DriverId}.");
            }
            return SchedulesOfDriver;
        }

        private List<GetScheduleDTO> MapScheduleToGetScheduleDTO(List<Schedule> schedules)
        {
            List<GetScheduleDTO> getScheduleDTOs = new List<GetScheduleDTO>();
            foreach(var schedule in schedules)
            {
                GetScheduleDTO getScheduleDTO = new GetScheduleDTO();
                getScheduleDTO.Id = schedule.Id;
                getScheduleDTO.BusNumber = schedule.BusNumber;
                getScheduleDTO.RouteId = schedule.RouteId;
                getScheduleDTO.DateTimeOfDeparture = schedule.DateTimeOfDeparture;
                getScheduleDTO.DateTimeOfArrival = schedule.DateTimeOfArrival;
                getScheduleDTOs.Add(getScheduleDTO);
            }
            return getScheduleDTOs;
        }
    }
}
