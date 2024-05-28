using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories.Classes
{
    public class BusWithSeatsRepository : BaseRepository<string, Bus>
    {
        public BusWithSeatsRepository(BusBookingContext context) : base(context) 
        {}

        public virtual async Task<Bus> Add(Bus entity)
        {
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationCustomException();
            }
        }

        public virtual async Task<IList<Bus>> GetAll()
        {
            var items = _context.Buses.Include(x => x.SeatsInBus).ToList();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException("No entities of type Bus are found.");
            }
            return items;
        }

        public override async Task<Bus> Delete(string key)
        {
            try
            {
                var item = await GetById(key);
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
        }

        public override async Task<Bus> GetById(string key)
        {
            var item = _context.Buses.Include(x => x.SeatsInBus).ToList().FirstOrDefault(x => x.BusNumber == key);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type Bus with BusNumber = {key} not found.");
            return item;
        }

        public override async Task<Bus> Update(Bus entity, string key)
        {
            try
            {
                await GetById(key);
                _context.Update(entity);
                int result = await _context.SaveChangesAsync();
                return entity;
            }
            catch (EntityNotFoundException) { throw; }
        }
    }
}
