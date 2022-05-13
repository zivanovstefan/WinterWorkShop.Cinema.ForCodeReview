using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.API.TokenServiceExtensions;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.Swagger;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WinterWorkShop.Cinema.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CinemaContext>(options =>
            {
                options
                .UseNpgsql(Configuration.GetConnectionString("CinemaConnection"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = Configuration["Jwt:Issuer"],
                          ValidAudience = Configuration["Jwt:Audience"],
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                      };
                    });
            services.AddControllers();
            services.AddOpenApiDocument();

            //// JWT token
            //services.AddJwtBearerAuthentication(Configuration);

            // Repositories
            services.AddTransient<IMoviesRepository, MoviesRepository>();
            services.AddTransient<IProjectionsRepository, ProjectionsRepository>();
            services.AddTransient<IAuditoriumsRepository, AuditoriumsRepository>();
            services.AddTransient<ICinemasRepository, CinemasRepository>();
            services.AddTransient<ISeatsRepository, SeatsRepository>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddScoped<ITicketsRepository, TicketRepository>();
            services.AddScoped<ISeatTicketRepository, SeatTicketRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IMovieTagsRepository, MovieTagsRepository>();

            // Business Logic
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IProjectionService, ProjectionService>();
            services.AddTransient<IAuditoriumService, AuditoriumService>();
            services.AddTransient<ICinemaService, CinemaService>();
            services.AddTransient<ISeatService, SeatService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILevi9PaymentService, Levi9PaymentService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ISeatTicketService, SeatTicketService>();
            services.AddScoped<ITagService, TagService>();

            // Allow Cors for client app
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    corsBuilder => corsBuilder.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseOpenApi();

            app.UseSwaggerUi3();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
