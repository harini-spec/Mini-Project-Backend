using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs.Schedule;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class BusService : IBusService
    {
        private readonly IRepository<string, Bus> _busRepo;


        public BusService(IRepository<string, Bus> BusRepository)
        {
            _busRepo = BusRepository;
        }


        // Checks if bus already booked during a specific time period. True if booked, false if not
        public bool CheckIfBusAlreadyBooked(List<Schedule> schedules, AddScheduleDTO addScheduleDTO)
        {
            foreach (var schedule in schedules)
            {
                if (schedule.BusNumber == addScheduleDTO.BusNumber)
                {
                    if ((addScheduleDTO.DateTimeOfDeparture >= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfArrival) ||
                        (addScheduleDTO.DateTimeOfArrival >= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfArrival) ||
                        (addScheduleDTO.DateTimeOfDeparture <= schedule.DateTimeOfDeparture && addScheduleDTO.DateTimeOfArrival >= schedule.DateTimeOfArrival))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        // Add bus with seats
        public async Task<AddBusDTO> AddBus(AddBusDTO InputBus)
        {
            if(InputBus.SeatsInBus.Count() == InputBus.TotalSeats)
            {
                Bus bus = MapAddBusDTOToBus(InputBus);
                List<Seat> seatsInGivenBus = MapAddSeatsInputDTOToSeatList(InputBus.BusNumber, InputBus.SeatsInBus);
                bus.SeatsInBus = seatsInGivenBus;
                await _busRepo.Add(bus);
                return InputBus;
            }
            throw new DataDoesNotMatchException();
        }


        public async Task<Bus> GetBusByBusNumber(string BusNumber)
        {
            return await _busRepo.GetById(BusNumber);
        }


        // Map AddBusDTO to Bus
        private Bus MapAddBusDTOToBus(AddBusDTO inputBus)
        {
            Bus bus = new Bus();
            bus.BusNumber = inputBus.BusNumber;
            bus.TotalSeats = inputBus.TotalSeats;
            return bus;
        }


        // Map AddSeatsInputDTO List DTO to Seat List 
        private List<Seat> MapAddSeatsInputDTOToSeatList(string BusNumber, IList<AddSeatsInputDTO> seatsInBus)
        {
            List<Seat> seats = new List<Seat>();
            foreach(var InputSeat in seatsInBus)
            {
                Seat seat = new Seat();
                seat.BusNumber = BusNumber;
                seat.SeatNumber = InputSeat.SeatNumber;
                seat.SeatNumber = InputSeat.SeatNumber;
                seat.SeatType = InputSeat.SeatType;
                seat.SeatPrice = InputSeat.SeatPrice;
                seats.Add(seat);
            }
            return seats;
        }
    }
}
