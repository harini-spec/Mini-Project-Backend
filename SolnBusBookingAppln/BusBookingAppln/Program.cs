using BusBookingAppln.Contexts;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BusBookingAppln
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddLogging(l => l.AddLog4Net());
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey:JWT"]))
                    };

                });

            #region CORS
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("MyCors", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            #endregion

            #region contexts
            builder.Services.AddDbContext<BusBookingContext>(
               options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))
               );
            #endregion

            #region repositories
            builder.Services.AddScoped<IRepository<string, Bus>, BusWithSeatsRepository>();
            builder.Services.AddScoped<IRepository<int, Driver>, DriverWithScheduleRepository>();
            builder.Services.AddScoped<IRepository<int, DriverDetail>, MainRepository<int, DriverDetail>>();
            builder.Services.AddScoped<IRepository<int, Feedback>, FeedbackWithTicketRepository>();
            builder.Services.AddScoped<IRepository<string, Payment>, MainRepository<string, Payment>>();
            builder.Services.AddScoped<IRepository<string, Refund>, MainRepository<string, Refund>>();
            builder.Services.AddScoped<IRepository<int, Reward>, MainRepository<int, Reward>>();
            builder.Services.AddScoped<IRepository<int, Models.DBModels.Route>, RouteWithRouteDetailRepository>();
            builder.Services.AddScoped<IRepository<int, Schedule>, MainRepository<int, Schedule>>();
            builder.Services.AddScoped<IRepository<int, Seat>, MainRepository<int, Seat>>();
            builder.Services.AddScoped<IRepository<int, Ticket>, TicketWithTicketDetailsRepository>();
            builder.Services.AddScoped<IRepository<int, User>, MainRepository<int, User>>();
            builder.Services.AddScoped<IRepository<int, UserDetail>, MainRepository<int, UserDetail>>();
            builder.Services.AddScoped<IRepositoryCompositeKey<int, int, RouteDetail>, RouteDetailRepository>();
            builder.Services.AddScoped<IRepositoryCompositeKey<int, int, TicketDetail>, TicketDetailRepository>();
            #endregion

            #region services 
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAdminService, DriverAccountService>();
            builder.Services.AddScoped<IDriverService, DriverService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBusService, BusService>();
            builder.Services.AddScoped<IRouteService, RouteService>();
            builder.Services.AddScoped<IRewardService, RewardService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<ISeatService, SeatService>(); 
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ISeatAvailability, SeatAvailabilityService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<ICustomerService, CustomerAccountService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            #endregion



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("MyCors");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
