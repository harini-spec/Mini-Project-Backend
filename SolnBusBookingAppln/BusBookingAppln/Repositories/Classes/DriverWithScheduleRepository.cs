﻿using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories.Classes
{
    public class DriverWithScheduleRepository : BaseRepository<int, Driver>
    {

        public DriverWithScheduleRepository(BusBookingContext context) : base(context) { }

        public override async Task<IList<Driver>> GetAll()
        {
            var items = _context.Drivers.Include(x => x.SchedulesForDriver).ToList();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException("No entities of type Driver are found.");
            }
            return items;
        }

        public override async Task<Driver> Delete(int key)
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

        public override async Task<Driver> GetById(int key)
        {
            var item = _context.Drivers.Include(x => x.SchedulesForDriver).ToList().FirstOrDefault(x => x.Id == key);
            if (item == null)
            {
                throw new EntityNotFoundException($"Entity of type Driver with ID {key} not found.");
            }
            return item;
        }

        public override async Task<Driver> Update(Driver entity, int key)
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
