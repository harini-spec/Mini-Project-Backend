using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories.Classes
{
    public class RouteDetailRepository : IRepositoryCompositeKey<int, int, RouteDetail>
    {
        public readonly BusBookingContext _context;
        public RouteDetailRepository(BusBookingContext context)
        {
            _context = context;
        }

        public async Task<RouteDetail> Add(RouteDetail entity)
        {
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (InvalidOperationException ioe)
            {
                throw new InvalidOperationCustomException();
            }

        }

        public virtual async Task<IList<RouteDetail>> GetAll()
        {
            var items = await _context.RouteDetails.ToListAsync();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException($"No entities of type {typeof(RouteDetail).Name} are found.");
            }
            return items;
        }

        public async Task<RouteDetail> Delete(int RouteId, int StopNumber)
        {
            try
            {
                var item = await GetById(RouteId, StopNumber);
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch(EntityNotFoundException)
            {
                throw;
            }
        }

        public virtual async Task<RouteDetail> GetById(int RouteId, int StopNumber)
        {
            var item = await _context.RouteDetails.FirstOrDefaultAsync(rd => rd.RouteId == RouteId && rd.StopNumber == StopNumber);
            if (item == null)
            {
                throw new EntityNotFoundException($"Entity of type {typeof(RouteDetail).Name} with RouteId = {RouteId} and StopNumber = {StopNumber} not found.");
            }
            return item;
        }

        public async Task<RouteDetail> Update(RouteDetail entity)
        {
            try
            {
                var item = await GetById(entity.RouteId, entity.StopNumber);
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (EntityNotFoundException) 
            {
                throw;
            }
        }
    }
}
