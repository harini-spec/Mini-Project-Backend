using BusBookingAppln.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Contexts
{
    public class BusBookingContext : DbContext
    {
        public BusBookingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Models.DBModels.Route> Routes { get; set; }    
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<RouteDetail> RouteDetails { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Seat> Seats { get; set; }  
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketDetail> TicketDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouteDetail>().HasKey(rd => new { rd.RouteId, rd.StopNumber });
            modelBuilder.Entity<TicketDetail>().HasKey(td => new { td.TicketId, td.SeatId });

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.TicketPaidFor)
                .WithMany(t => t.PaymentsForTicket)
                .HasForeignKey(p => p.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.TicketRefundedFor)
                .WithMany(t => t.RefundsForTicket)
                .HasForeignKey(r => r.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RouteDetail>()
                .HasOne(rd => rd.ForRoute)
                .WithMany(r => r.RouteStops)
                .HasForeignKey(rd => rd.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.ScheduledRoute)
                .WithMany(r => r.SchedulesInThisRoute)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.ScheduledBus)
                .WithMany(b => b.SchedulesForBus)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.ScheduledDriver)
                .WithMany(d => d.SchedulesForDriver)
                .HasForeignKey(s => s.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.SeatInBus)
                .WithMany(b => b.SeatsInBus)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.UserBooking)
                .WithMany(u => u.TicketsAdded)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.BookedSchedule)
                .WithMany(s => s.TicketsAdded)
                .HasForeignKey(t => t.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketDetail>()
                .HasOne(td => td.TicketDetailsForTicket)
                .WithMany(t => t.TicketDetails)
                .HasForeignKey(td => td.TicketId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
