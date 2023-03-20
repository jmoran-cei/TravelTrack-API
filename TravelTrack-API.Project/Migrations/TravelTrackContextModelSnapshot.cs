﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TravelTrack_API.DbContexts;

#nullable disable

namespace TravelTrack_API.Migrations
{
    [DbContext(typeof(TravelTrackContext))]
    partial class TravelTrackContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TravelTrack_API.Domain.B2CUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("B2CUser");

                    b.HasData(
                        new
                        {
                            Id = "3cf84869-e3b3-4ff4-9421-0c5271e6cc92",
                            Username = "jmoran@ceiamerica.com"
                        },
                        new
                        {
                            Id = "7241d571-59ec-40f7-8730-84de1ff982d6",
                            Username = "testuser@test.test"
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.Destination", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Destinations");

                    b.HasData(
                        new
                        {
                            Id = "ChIJw4OtEaZjDowRZCw_jCcczqI",
                            City = "Zemi Beach",
                            Country = "Anguilla",
                            Region = "West End"
                        },
                        new
                        {
                            Id = "ChIJASFVO5VoAIkRGJbQtRWxD7w",
                            City = "Myrtle Beach",
                            Country = "United States",
                            Region = "South Carolina"
                        },
                        new
                        {
                            Id = "ChIJdySo3EJ6_ogRa-zhruD3-jU",
                            City = "Charleston",
                            Country = "United States",
                            Region = "South Carolina"
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AddedByUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Alt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TripId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TripId");

                    b.ToTable("Photos");

                    b.HasData(
                        new
                        {
                            Id = new Guid("11223344-5566-7788-99aa-bbccddeeff00"),
                            AddedByUser = "jmoran@ceiamerica.com",
                            Alt = "1-sample-trip-img.jpg",
                            FileName = "1-sample-trip-img.jpg",
                            FileType = "image/jpg",
                            Path = "https://bootcampjmoranstorage.blob.core.windows.net/trip-photos/1-sample-trip-img.jpg",
                            TripId = 1L
                        },
                        new
                        {
                            Id = new Guid("21223344-5566-7788-99aa-bbccddeeff00"),
                            AddedByUser = "jmoran@ceiamerica.com",
                            Alt = "1-travel-track-readme-img.jpg",
                            FileName = "1-travel-track-readme-img.jpg",
                            FileType = "image/jpg",
                            Path = "https://bootcampjmoranstorage.blob.core.windows.net/trip-photos/1-travel-track-readme-img.jpg",
                            TripId = 1L
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.ToDo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Complete")
                        .HasColumnType("bit");

                    b.Property<string>("Task")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TripId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TripId");

                    b.ToTable("ToDo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Complete = true,
                            Task = "buy new swim trunks",
                            TripId = 1L
                        },
                        new
                        {
                            Id = 2,
                            Complete = true,
                            Task = "pack beach towels",
                            TripId = 1L
                        },
                        new
                        {
                            Id = 3,
                            Complete = true,
                            Task = "buy new swim trunks",
                            TripId = 2L
                        },
                        new
                        {
                            Id = 4,
                            Complete = true,
                            Task = "buy new swim trunks",
                            TripId = 2L
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.Trip", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImgURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Trips");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                            EndDate = new DateTime(2022, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ImgURL = "assets/images/trips/anguila1.jpg",
                            StartDate = new DateTime(2022, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Brothers' Anguila Trip"
                        },
                        new
                        {
                            Id = 2L,
                            Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                            EndDate = new DateTime(2022, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ImgURL = "assets/images/trips/myrtlebeach1.jpg",
                            StartDate = new DateTime(2022, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Myrtle Beach and Charleston Family Vacay 2022"
                        },
                        new
                        {
                            Id = 3L,
                            Details = "",
                            EndDate = new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ImgURL = "assets/images/trips/myrtlebeach1.jpg",
                            StartDate = new DateTime(2024, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Another Test Trip"
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.TripB2CUser", b =>
                {
                    b.Property<long>("TripId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TripId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("TripB2CMembers");

                    b.HasData(
                        new
                        {
                            TripId = 1L,
                            UserId = "3cf84869-e3b3-4ff4-9421-0c5271e6cc92"
                        },
                        new
                        {
                            TripId = 1L,
                            UserId = "7241d571-59ec-40f7-8730-84de1ff982d6"
                        },
                        new
                        {
                            TripId = 2L,
                            UserId = "3cf84869-e3b3-4ff4-9421-0c5271e6cc92"
                        },
                        new
                        {
                            TripId = 2L,
                            UserId = "7241d571-59ec-40f7-8730-84de1ff982d6"
                        },
                        new
                        {
                            TripId = 3L,
                            UserId = "3cf84869-e3b3-4ff4-9421-0c5271e6cc92"
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.TripDestination", b =>
                {
                    b.Property<long>("TripId")
                        .HasColumnType("bigint");

                    b.Property<string>("DestinationId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TripId", "DestinationId");

                    b.HasIndex("DestinationId");

                    b.ToTable("TripDestinations");

                    b.HasData(
                        new
                        {
                            TripId = 1L,
                            DestinationId = "ChIJw4OtEaZjDowRZCw_jCcczqI"
                        },
                        new
                        {
                            TripId = 2L,
                            DestinationId = "ChIJASFVO5VoAIkRGJbQtRWxD7w"
                        },
                        new
                        {
                            TripId = 2L,
                            DestinationId = "ChIJdySo3EJ6_ogRa-zhruD3-jU"
                        },
                        new
                        {
                            TripId = 3L,
                            DestinationId = "ChIJdySo3EJ6_ogRa-zhruD3-jU"
                        },
                        new
                        {
                            TripId = 3L,
                            DestinationId = "ChIJASFVO5VoAIkRGJbQtRWxD7w"
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.TripUser", b =>
                {
                    b.Property<long>("TripId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TripId", "Username");

                    b.HasIndex("Username");

                    b.ToTable("TripMembers");

                    b.HasData(
                        new
                        {
                            TripId = 1L,
                            Username = "jmoran@ceiamerica.com"
                        },
                        new
                        {
                            TripId = 1L,
                            Username = "dummyuser@dummy.dum"
                        },
                        new
                        {
                            TripId = 2L,
                            Username = "jmoran@ceiamerica.com"
                        },
                        new
                        {
                            TripId = 2L,
                            Username = "dummyuser@dummy.dum"
                        },
                        new
                        {
                            TripId = 2L,
                            Username = "fakeuser@fakey.fake"
                        },
                        new
                        {
                            TripId = 3L,
                            Username = "jmoran@ceiamerica.com"
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Username");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Username = "jmoran@ceiamerica.com",
                            FirstName = "Jonathan",
                            LastName = "Moran",
                            Password = "P@ssw0rd"
                        },
                        new
                        {
                            Username = "dummyuser@dummy.dum",
                            FirstName = "Dummy",
                            LastName = "User",
                            Password = "P@ssw0rd"
                        },
                        new
                        {
                            Username = "fakeuser@fakey.fake",
                            FirstName = "Fake",
                            LastName = "User",
                            Password = "P@ssw0rd"
                        });
                });

            modelBuilder.Entity("TravelTrack_API.Domain.Photo", b =>
                {
                    b.HasOne("TravelTrack_API.Domain.Trip", "Trip")
                        .WithMany("Photos")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.ToDo", b =>
                {
                    b.HasOne("TravelTrack_API.Domain.Trip", "Trip")
                        .WithMany("ToDo")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.TripB2CUser", b =>
                {
                    b.HasOne("TravelTrack_API.Domain.Trip", "Trip")
                        .WithMany("B2CMembers")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TravelTrack_API.Domain.B2CUser", "B2CUser")
                        .WithMany("Trips")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("B2CUser");

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.TripDestination", b =>
                {
                    b.HasOne("TravelTrack_API.Domain.Destination", "Destination")
                        .WithMany("Trips")
                        .HasForeignKey("DestinationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TravelTrack_API.Domain.Trip", "Trip")
                        .WithMany("Destinations")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Destination");

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.TripUser", b =>
                {
                    b.HasOne("TravelTrack_API.Domain.Trip", "Trip")
                        .WithMany("Members")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TravelTrack_API.Domain.User", "User")
                        .WithMany("Trips")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trip");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.B2CUser", b =>
                {
                    b.Navigation("Trips");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.Destination", b =>
                {
                    b.Navigation("Trips");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.Trip", b =>
                {
                    b.Navigation("B2CMembers");

                    b.Navigation("Destinations");

                    b.Navigation("Members");

                    b.Navigation("Photos");

                    b.Navigation("ToDo");
                });

            modelBuilder.Entity("TravelTrack_API.Domain.User", b =>
                {
                    b.Navigation("Trips");
                });
#pragma warning restore 612, 618
        }
    }
}
