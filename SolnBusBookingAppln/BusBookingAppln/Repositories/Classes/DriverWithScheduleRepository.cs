using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories.Classes
{
    public class DriverWithScheduleRepository : IRepository<int, Driver>
    {
        public readonly BusBookingContext _context;

        public DriverWithScheduleRepository(BusBookingContext context)
        {
            _context = context;
        }

        public virtual async Task<Driver> Add(Driver entity)
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

        public virtual async Task<IList<Driver>> GetAll()
        {
            var items = _context.Drivers.Include(x => x.SchedulesForDriver).ToList();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException("No entities of type Driver are found.");
            }
            return items;
        }

        public virtual async Task<Driver> Delete(int key)
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

        public virtual async Task<Driver> GetById(int key)
        {
            var item = _context.Drivers.Include(x => x.SchedulesForDriver).ToList().FirstOrDefault(x => x.Id == key);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type Driver with ID {key} not found.");
            return item;
        }

        public virtual async Task<Driver> Update(Driver entity, int key)
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
