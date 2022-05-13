using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    public class CinemaContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<CinemaEntity> Cinemas { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<SeatTicket> SeatTickets { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<MovieTag> MovieTags { get; set; }

        public CinemaContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// <summary>
            /// Composite key for SeatTicket
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<SeatTicket>()
                .HasKey(x => new { x.TicketId, x.SeatId });

            /// <summary>
            /// Composite key for MovieTag
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<MovieTag>()
                .HasKey(x => new { x.MovieId, x.TagId });

            /// <summary>
            /// Seat -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Seat>()
                .HasOne(x => x.Auditorium)
                .WithMany(x => x.Seats)
                .HasForeignKey(x => x.AuditoriumId)
                .IsRequired();

            /// <summary>
            /// Auditorium -> Seat relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()
                .HasMany(x => x.Seats)
                .WithOne(x => x.Auditorium)
                .IsRequired();


            /// <summary>
            /// Cinema -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<CinemaEntity>()
                .HasMany(x => x.Auditoriums)
                .WithOne(x => x.Cinema)
                .IsRequired();
            
            /// <summary>
            /// Auditorium -> Cinema relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()
                .HasOne(x => x.Cinema)
                .WithMany(x => x.Auditoriums)
                .HasForeignKey(x => x.CinemaId)
                .IsRequired();


            /// <summary>
            /// Auditorium -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()               
               .HasMany(x => x.Projections)
               .WithOne(x => x.Auditorium)
               .IsRequired();

            /// <summary>
            /// Projection -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasOne(x => x.Auditorium)
                .WithMany(x => x.Projections)
                .HasForeignKey(x => x.AuditoriumId)
                .IsRequired();


            /// <summary>
            /// Projection -> Movie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.Projections)
                .HasForeignKey(x => x.MovieId)
                .IsRequired();

            /// <summary>
            /// Movie -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Movie>()
                .HasMany(x => x.Projections)
                .WithOne(x => x.Movie)
                .IsRequired();
            
            //PROJECTION TICKET
            /// <summary>
            /// Projection -> Tickets relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasMany(x => x.Tickets)
                .WithOne(x => x.Projection)
                .IsRequired();

            /// <summary>
            /// Ticket -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Ticket>()
                .HasOne(x => x.Projection)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.ProjectionId)
                .IsRequired();

            //USER TICKET
            /// <summary>
            /// User -> Tickets relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<User>()
                .HasMany(x => x.Tickets)
                .WithOne(x => x.User)
                .IsRequired();

            /// <summary>
            /// Ticket -> User relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Ticket>()
                .HasOne(x => x.User)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            /// <summary>
            /// Ticket -> SeatTicket relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Ticket>()
                .HasMany(x => x.SeatTickets)
                .WithOne(x => x.Ticket)
                .IsRequired();

            /// <summary>
            /// SeatTicket -> Ticket relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<SeatTicket>()
                .HasOne(x => x.Ticket)
                .WithMany(x => x.SeatTickets)
                .HasForeignKey(x => x.TicketId)
                .IsRequired();

            /// <summary>
            /// Seat -> SeatTicket relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Seat>()
                .HasMany(x => x.SeatTickets)
                .WithOne(x => x.Seat)
                .IsRequired();

            /// <summary>
            /// SeatTicket -> Seat relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<SeatTicket>()
                .HasOne(x => x.Seat)
                .WithMany(x => x.SeatTickets)
                .HasForeignKey(x => x.SeatId)
                .IsRequired();

            /// <summary>
            /// Movie -> MovieTag relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Movie>()
                .HasMany(x => x.MovieTags)
                .WithOne(x => x.Movie)
                .IsRequired();

            /// <summary>
            /// MovieTag -> Movie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<MovieTag>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.MovieTags)
                .HasForeignKey(x => x.MovieId)
                .IsRequired();

            /// <summary>
            /// MovieTag -> Tags relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<MovieTag>()
                .HasOne(x => x.Tag)
                .WithMany(x => x.MovieTags)
                .HasForeignKey(x => x.TagId)
                .IsRequired();

            /// <summary>
            /// Tags -> MovieTag relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Tag>()
                .HasMany(x => x.MovieTags)
                .WithOne(x => x.Tag)
                .IsRequired();

        }
    }
}
