﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WinterWorkShop.Cinema.Data;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    [DbContext(typeof(CinemaContext))]
    [Migration("20220504100945_HasOscar_property_added")]
    partial class HasOscar_property_added
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Auditorium", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuditName")
                        .HasColumnType("text")
                        .HasColumnName("AuditoriumName");

                    b.Property<int>("CinemaId")
                        .HasColumnType("integer")
                        .HasColumnName("cinemaId");

                    b.HasKey("Id");

                    b.HasIndex("CinemaId");

                    b.ToTable("auditorium");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.CinemaEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("cinema");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.MovieTag", b =>
                {
                    b.Property<Guid>("MovieId")
                        .HasColumnType("uuid")
                        .HasColumnName("movieId");

                    b.Property<int>("TagId")
                        .HasColumnType("integer")
                        .HasColumnName("tagId");

                    b.HasKey("MovieId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("movieTags");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.SeatTicket", b =>
                {
                    b.Property<Guid>("TicketId")
                        .HasColumnType("uuid")
                        .HasColumnName("ticketId");

                    b.Property<Guid>("SeatId")
                        .HasColumnType("uuid")
                        .HasColumnName("seatId");

                    b.Property<DateTime>("ProjectionTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("TicketId", "SeatId");

                    b.HasIndex("SeatId");

                    b.ToTable("SeatTicket");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("tagName");

                    b.HasKey("Id");

                    b.ToTable("tag");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<Guid>("ProjectionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProjectionId");

                    b.HasIndex("UserId");

                    b.ToTable("ticket");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Current")
                        .HasColumnType("boolean");

                    b.Property<bool>("HasOscar")
                        .HasColumnType("boolean");

                    b.Property<double?>("Rating")
                        .HasColumnType("double precision");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("movie");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Projection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AuditoriumId")
                        .HasColumnType("integer")
                        .HasColumnName("auditorium_id");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("MovieId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AuditoriumId");

                    b.HasIndex("MovieId");

                    b.ToTable("projection");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Seat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AuditoriumId")
                        .HasColumnType("integer")
                        .HasColumnName("auditorium_id");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("Row")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AuditoriumId");

                    b.ToTable("seat");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("BonusPoints")
                        .HasColumnType("integer");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean")
                        .HasColumnName("is_admin");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text")
                        .HasColumnName("userName");

                    b.HasKey("Id");

                    b.ToTable("user");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Auditorium", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.CinemaEntity", "Cinema")
                        .WithMany("Auditoriums")
                        .HasForeignKey("CinemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cinema");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.MovieTag", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Movie", "Movie")
                        .WithMany("MovieTags")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WinterWorkShop.Cinema.Data.Entities.Tag", "Tag")
                        .WithMany("MovieTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.SeatTicket", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Seat", "Seat")
                        .WithMany("SeatTickets")
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WinterWorkShop.Cinema.Data.Entities.Ticket", "Ticket")
                        .WithMany("SeatTickets")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Seat");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Ticket", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Projection", "Projection")
                        .WithMany("Tickets")
                        .HasForeignKey("ProjectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WinterWorkShop.Cinema.Data.User", "User")
                        .WithMany("Tickets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Projection");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Projection", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Auditorium", "Auditorium")
                        .WithMany("Projections")
                        .HasForeignKey("AuditoriumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WinterWorkShop.Cinema.Data.Movie", "Movie")
                        .WithMany("Projections")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Auditorium");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Seat", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Auditorium", "Auditorium")
                        .WithMany("Seats")
                        .HasForeignKey("AuditoriumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Auditorium");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Auditorium", b =>
                {
                    b.Navigation("Projections");

                    b.Navigation("Seats");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.CinemaEntity", b =>
                {
                    b.Navigation("Auditoriums");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Tag", b =>
                {
                    b.Navigation("MovieTags");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Ticket", b =>
                {
                    b.Navigation("SeatTickets");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Movie", b =>
                {
                    b.Navigation("MovieTags");

                    b.Navigation("Projections");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Projection", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Seat", b =>
                {
                    b.Navigation("SeatTickets");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.User", b =>
                {
                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
