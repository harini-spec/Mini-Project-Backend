using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
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

        private Bus MapAddBusDTOToBus(AddBusDTO inputBus)
        {
            Bus bus = new Bus();
            bus.BusNumber = inputBus.BusNumber;
            bus.TotalSeats = inputBus.TotalSeats;
            return bus;
        }
    }
}
