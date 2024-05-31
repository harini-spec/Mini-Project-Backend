using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories.Classes
{
    public class RouteWithRouteDetailRepository : BaseRepository<int, Models.DBModels.Route>
    {
        public RouteWithRouteDetailRepository(BusBookingContext context) : base(context)
        {
        }

        public override async Task<IList<Models.DBModels.Route>> GetAll()
        {
            var items = await _context.Routes.Include(x => x.RouteStops).ToListAsync();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException($"No entities of type Route are found.");
            }
            return items;
        }

        public override async Task<Models.DBModels.Route> Delete(int key)
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

        public override async Task<Models.DBModels.Route> GetById(int key)
        {
            var item = _context.Routes.Include(x => x.RouteStops).ToList().FirstOrDefault(x => x.Id == key);
            if (item == null)
            {
                throw new EntityNotFoundException($"Entity of type Route with Id = {key} not found.");
            }
            return item;
        }

        public override async Task<Models.DBModels.Route> Update(Models.DBModels.Route entity, int key)
        {
            try
            {
                await GetById(key);
                _context.Update(entity);
                int result = await _context.SaveChangesAsync();
                return entity;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
        }
    }
}
