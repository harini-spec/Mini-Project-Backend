using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class SeatService : ISeatService
    {
        private readonly IRepository<int, Seat> _SeatRepo;
        private readonly ILogger<SeatService> _logger;
        private readonly IRepository<string, Bus> _busRepositoryWithSeats;

        public SeatService(IRepository<int, Seat> seatRepo, ILogger<SeatService> logger, IRepository<string, Bus> busRepositoryWithSeats)
        {
            _SeatRepo = seatRepo;
            _logger = logger;
            _busRepositoryWithSeats = busRepositoryWithSeats;
        }

        public async Task<List<GetSeatsDTO>> GetSeatsOfBus(string BusNumber)
        {
            try
            {
                Bus bus = await _busRepositoryWithSeats.GetById(BusNumber);
                if (bus.SeatsInBus.Count == 0)
                    throw new NoItemsFoundException("No Seats found!");
                else
                {
                    List<GetSeatsDTO> seatsDTOList = new List<GetSeatsDTO>();
                    foreach(var seat in bus.SeatsInBus)
                    {
                        seatsDTOList.Add(MapSeatToGetSeatsDTO(seat));
                    }
                    return seatsDTOList;
                }
            }
            catch (EntityNotFoundException enf)
            {
                _logger.LogError(enf.Message);
                throw;
            }
            catch(NoItemsFoundException nif)
            {
                _logger.LogError(nif.Message);
                throw;
            }
        }

        public async Task<GetSeatsDTO> GetSeatById(int seatId)
        {
            try
            {
                Seat seat = await _SeatRepo.GetById(seatId);
                GetSeatsDTO getSeat = MapSeatToGetSeatsDTO(seat);
                return getSeat;
            }
            catch(EntityNotFoundException enf)
            {
                _logger.LogError(enf.Message);
                throw;
            }
        }

        #region Mappers
        private GetSeatsDTO MapSeatToGetSeatsDTO(Seat seat)
        {
            GetSeatsDTO getSeatsDTO = new GetSeatsDTO();
            getSeatsDTO.Id = seat.Id;
            getSeatsDTO.SeatNumber = seat.SeatNumber;
            getSeatsDTO.SeatType = seat.SeatType;
            getSeatsDTO.SeatPrice = seat.SeatPrice;
            return getSeatsDTO;
        }
        #endregion
    }
}
