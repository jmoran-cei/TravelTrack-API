using Microsoft.EntityFrameworkCore;
using TravelTrack_API.Domain;

namespace TravelTrack_API.DbContexts;

public class TravelTrackContext : DbContext
{
    public DbSet<Trip> Trips { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public TravelTrackContext(DbContextOptions<TravelTrackContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder bldr)
    {
        // configure TripDestination join (many-to-many)
        bldr.Entity<TripDestination>()
            .HasKey(td => new { td.TripId, td.DestinationId });

        bldr.Entity<TripDestination>()
            .HasOne(td => td.Trip)
            .WithMany(t => t.Destinations)
            .HasForeignKey(td => td.TripId);

        bldr.Entity<TripDestination>()
            .HasOne(td => td.Destination)
            .WithMany(d => d.Trips)
            .HasForeignKey(td => td.DestinationId);

        // configure TripUser join (many-to-many)
        bldr.Entity<TripUser>()
            .HasKey(d => new { d.TripId, d.Username });

        bldr.Entity<TripUser>()
            .HasOne(tu => tu.Trip)
            .WithMany(t => t.Members)
            .HasForeignKey(tu => tu.TripId);

        bldr.Entity<TripUser>()
            .HasOne(tu => tu.User)
            .WithMany(u => u.Trips)
            .HasForeignKey(tu => tu.Username);

        // configure ToDo (one-to-many)
        bldr.Entity<ToDo>()
            .HasOne(t => t.Trip)
            .WithMany(t => t.ToDo)
            .HasForeignKey(t => t.TripId);

        var trips = new Trip[]
        {
            new Trip
            {
                Id = 1,
                Title = "Brothers' Anguila Trip",
                Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                StartDate = new DateTime(2022, 3, 15),
                EndDate = new DateTime(2022, 3, 20),
                ImgURL = "assets/images/trips/anguila1.jpg",
            },
            new Trip
            {
                Id = 2,
                Title = "Myrtle Beach and Charleston Family Vacay 2022",
                Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                StartDate = new DateTime(2022, 5, 27),
                EndDate = new DateTime(2022, 6, 5),
                ImgURL = "assets/images/trips/myrtlebeach1.jpg",
            },
            new Trip
            {
                Id = 3,
                Title = "Another Test Trip",
                Details = "",
                StartDate = new DateTime(2024, 7, 19),
                EndDate = new DateTime(2024, 8, 1),
                ImgURL = "assets/images/trips/myrtlebeach1.jpg"
            }
        };

        var destinations = new Destination[]
        {
            new Destination { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" },
            new Destination { Id = "ChIJASFVO5VoAIkRGJbQtRWxD7w", City = "Myrtle Beach", Region = "South Carolina", Country = "United States" },
            new Destination { Id = "ChIJdySo3EJ6_ogRa-zhruD3-jU", City = "Charleston", Region = "South Carolina", Country = "United States" },
        };

        var tripDestinations = new TripDestination[]
        {
            new TripDestination { TripId = 1, DestinationId = "ChIJw4OtEaZjDowRZCw_jCcczqI" },
            new TripDestination { TripId = 2, DestinationId = "ChIJASFVO5VoAIkRGJbQtRWxD7w" },
            new TripDestination { TripId = 2, DestinationId = "ChIJdySo3EJ6_ogRa-zhruD3-jU" },
            new TripDestination { TripId = 3, DestinationId = "ChIJdySo3EJ6_ogRa-zhruD3-jU" },
            new TripDestination { TripId = 3, DestinationId = "ChIJASFVO5VoAIkRGJbQtRWxD7w" },
        };

        var tripMembers = new TripUser[]
        {
            new TripUser { TripId = 1, Username = "jmoran@ceiamerica.com" },
            new TripUser { TripId = 1, Username = "dummyuser@dummy.dum" },
            new TripUser { TripId = 2, Username = "jmoran@ceiamerica.com" },
            new TripUser { TripId = 2, Username = "dummyuser@dummy.dum" },
            new TripUser { TripId = 2, Username = "fakeuser@fakey.fake" },
            new TripUser { TripId = 3, Username = "jmoran@ceiamerica.com" },
        };

        var toDo = new ToDo[]
        {
            new ToDo { Id = 1, TripId = 1, Task = "buy new swim trunks", Complete = true },
            new ToDo { Id = 2, TripId = 1, Task = "pack beach towels", Complete = true },
            new ToDo { Id = 3, TripId = 2, Task = "buy new swim trunks", Complete = true },
            new ToDo { Id = 4, TripId = 2, Task = "buy new swim trunks", Complete = true }
        };

        var users = new User[]
        {
            new User { Username = "jmoran@ceiamerica.com", Password = "P@ssw0rd", FirstName = "Jonathan", LastName = "Moran" },
            new User { Username = "dummyuser@dummy.dum", Password = "P@ssw0rd", FirstName = "Dummy", LastName = "User" },
            new User { Username = "fakeuser@fakey.fake", Password = "P@ssw0rd", FirstName = "Fake", LastName = "User" }
        };

        bldr.Entity<Trip>()
            .HasData(trips);

        bldr.Entity<User>()
            .HasData(users);

        bldr.Entity<TripUser>()
            .HasData(tripMembers);

        bldr.Entity<Destination>()
            .HasData(destinations);

        bldr.Entity<TripDestination>()
            .HasData(tripDestinations);

        bldr.Entity<ToDo>()
            .HasData(toDo);

        base.OnModelCreating(bldr);
    }
}