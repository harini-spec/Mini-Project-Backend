﻿// <auto-generated />
using System;
using BusBookingAppln.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BusBookingAppln.Migrations
{
    [DbContext(typeof(BusBookingContext))]
    partial class BusBookingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Bus", b =>
                {
                    b.Property<string>("BusNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TotalSeats")
                        .HasColumnType("int");

                    b.HasKey("BusNumber");

                    b.ToTable("Buses");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Driver", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("YearsOfExperience")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Drivers");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.DriverDetail", b =>
                {
                    b.Property<int>("DriverId")
                        .HasColumnType("int");

                    b.Property<byte[]>("PasswordEncrypted")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordHashKey")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DriverId");

                    b.ToTable("DriversDetails");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Feedback", b =>
                {
                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FeedbackDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("TicketId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Payment", b =>
                {
                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("DiscountPercentage")
                        .HasColumnType("real");

                    b.Property<float>("GSTPercentage")
                        .HasColumnType("real");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<float>("TotalAmount")
                        .HasColumnType("real");

                    b.HasKey("TransactionId");

                    b.HasIndex("TicketId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Refund", b =>
                {
                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("RefundAmount")
                        .HasColumnType("real");

                    b.Property<DateTime>("RefundDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.HasKey("TransactionId");

                    b.HasIndex("TicketId");

                    b.ToTable("Refunds");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Reward", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RewardPoints")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("Rewards");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Route", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Destination")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.RouteDetail", b =>
                {
                    b.Property<int>("RouteId")
                        .HasColumnType("int");

                    b.Property<int>("StopNumber")
                        .HasColumnType("int");

                    b.Property<string>("From_Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("To_Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("RouteId", "StopNumber");

                    b.ToTable("RouteDetails");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BusNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateTimeOfArrival")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTimeOfDeparture")
                        .HasColumnType("datetime2");

                    b.Property<int>("DriverId")
                        .HasColumnType("int");

                    b.Property<int>("RouteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BusNumber");

                    b.HasIndex("DriverId");

                    b.HasIndex("RouteId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Seat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BusNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("SeatPrice")
                        .HasColumnType("real");

                    b.Property<string>("SeatType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BusNumber");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateAndTimeOfAdding")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Total_Cost")
                        .HasColumnType("real");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("UserId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.TicketDetail", b =>
                {
                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<int>("SeatId")
                        .HasColumnType("int");

                    b.Property<int>("PassengerAge")
                        .HasColumnType("int");

                    b.Property<string>("PassengerGender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PassengerPhone")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<float>("SeatPrice")
                        .HasColumnType("real");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TicketId", "SeatId");

                    b.HasIndex("SeatId");

                    b.ToTable("TicketDetails");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.UserDetail", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<byte[]>("PasswordEncrypted")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordHashKey")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("UserDetails");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.DriverDetail", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Driver", "DriverDetailsForDriver")
                        .WithOne("DriverDetails")
                        .HasForeignKey("BusBookingAppln.Models.DBModels.DriverDetail", "DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DriverDetailsForDriver");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Feedback", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Ticket", "FeedbackForTicket")
                        .WithOne("FeedbackForRide")
                        .HasForeignKey("BusBookingAppln.Models.DBModels.Feedback", "TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FeedbackForTicket");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Payment", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Ticket", "TicketPaidFor")
                        .WithMany("PaymentsForTicket")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("TicketPaidFor");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Refund", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Ticket", "TicketRefundedFor")
                        .WithMany("RefundsForTicket")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("TicketRefundedFor");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Reward", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.User", "RewardsForUser")
                        .WithOne("RewardsOfUser")
                        .HasForeignKey("BusBookingAppln.Models.DBModels.Reward", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RewardsForUser");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.RouteDetail", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Route", "ForRoute")
                        .WithMany("RouteStops")
                        .HasForeignKey("RouteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ForRoute");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Schedule", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Bus", "ScheduledBus")
                        .WithMany("SchedulesForBus")
                        .HasForeignKey("BusNumber")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BusBookingAppln.Models.DBModels.Driver", "ScheduledDriver")
                        .WithMany("SchedulesForDriver")
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BusBookingAppln.Models.DBModels.Route", "ScheduledRoute")
                        .WithMany("SchedulesInThisRoute")
                        .HasForeignKey("RouteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ScheduledBus");

                    b.Navigation("ScheduledDriver");

                    b.Navigation("ScheduledRoute");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Seat", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Bus", "SeatInBus")
                        .WithMany("SeatsInBus")
                        .HasForeignKey("BusNumber")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SeatInBus");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Ticket", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Schedule", "BookedSchedule")
                        .WithMany("TicketsAdded")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BusBookingAppln.Models.DBModels.User", "UserBooking")
                        .WithMany("TicketsAdded")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BookedSchedule");

                    b.Navigation("UserBooking");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.TicketDetail", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.Seat", "SeatReserved")
                        .WithMany()
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusBookingAppln.Models.DBModels.Ticket", "TicketDetailsForTicket")
                        .WithMany("TicketDetails")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SeatReserved");

                    b.Navigation("TicketDetailsForTicket");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.UserDetail", b =>
                {
                    b.HasOne("BusBookingAppln.Models.DBModels.User", "UserDetailsForUser")
                        .WithOne("UserDetails")
                        .HasForeignKey("BusBookingAppln.Models.DBModels.UserDetail", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserDetailsForUser");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Bus", b =>
                {
                    b.Navigation("SchedulesForBus");

                    b.Navigation("SeatsInBus");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Driver", b =>
                {
                    b.Navigation("DriverDetails")
                        .IsRequired();

                    b.Navigation("SchedulesForDriver");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Route", b =>
                {
                    b.Navigation("RouteStops");

                    b.Navigation("SchedulesInThisRoute");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Schedule", b =>
                {
                    b.Navigation("TicketsAdded");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.Ticket", b =>
                {
                    b.Navigation("FeedbackForRide");

                    b.Navigation("PaymentsForTicket");

                    b.Navigation("RefundsForTicket");

                    b.Navigation("TicketDetails");
                });

            modelBuilder.Entity("BusBookingAppln.Models.DBModels.User", b =>
                {
                    b.Navigation("RewardsOfUser");

                    b.Navigation("TicketsAdded");

                    b.Navigation("UserDetails")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
