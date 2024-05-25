using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories
{
    public class BusRepository : BaseRepository<string, Bus>
    {
        public BusRepository(BusBookingContext context) : base(context)
        {
        }

        public async override Task<Bus> Delete(string key)
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

        public async override Task<Bus> GetById(string key)
        {
            var item = await _context.Buses.FirstOrDefaultAsync(b => b.BusNumber == key);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type {typeof(Bus).Name} with Bus Number {key} not found.");
            return item;
        }

        public async override Task<Bus> Update(Bus entity, string key)
        {
            try
            {
                var item = await GetById(key);
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (EntityNotFoundException) { throw; }
        }
    }
}
