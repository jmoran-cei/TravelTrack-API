using TravelTrack_API.Models;

namespace TravelTrack_API.Services;

public static class TripService
{
    static List<Trip> Trips { get; }
    static TripService()
    {
        Trips = new List<Trip>
        {
            new Trip
            {
                Id = 1,
                Title = "Brothers' Anguila Trip",
                Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                StartDate = new DateTime(2022, 3, 15),
                EndDate = new DateTime(2022, 3, 20),
                Destinations = new List<Destination>
                {
                    new Destination { DestinationId = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<User>
                {
                    new User { Username = "jmoran@ceiamerica.com", Password = "P@ssw0rd", FirstName = "Jonathan", LastName = "Moran" },
                    new User { Username = "dummyuser@dummy.dum", Password = "P@ssw0rd", FirstName = "Dummy", LastName = "User" },
                    new User { Username = "fakeyfake@fakey.fake", Password = "P@ssw0rd", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDo>
                {
                    new ToDo { Task = "pack clothes", Complete = false },
                    new ToDo { Task = "get snacks", Complete = true },
                    new ToDo { Task = "finish booking resort", Complete = true },
                    new ToDo { Task = "buy more toothpaste", Complete = false },
                },
                ImgURL = "assets/images/trips/anguila1.jpg"
            },
            new Trip
            {
                Id = 2,
                Title = "Myrtle Beach and Charleston Family Vacay 2022",
                Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                StartDate = new DateTime(2022, 5, 27),
                EndDate = new DateTime(2022, 6, 5),
                Destinations = new List<Destination>
                {
                    new Destination { DestinationId = "ChIJASFVO5VoAIkRGJbQtRWxD7w", City = "Myrtle Beach", Region = "South Carolina", Country = "United States" },
                    new Destination { DestinationId = "ChIJdySo3EJ6_ogRa-zhruD3-jU", City = "Charleston", Region = "South Carolina", Country = "United States" },
                },
                Members = new List<User>
                {
                    new User { Username = "jmoran@ceiamerica.com", Password = "P@ssw0rd", FirstName = "Jonathan", LastName = "Moran" },
                    new User { Username = "dummyuser@dummy.dum", Password = "P@ssw0rd", FirstName = "Dummy", LastName = "User" },
                    new User { Username = "fakeyfake@fakey.fake", Password = "P@ssw0rd", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDo>
                {
                    new ToDo { Task = "buy new swim trunks", Complete = true },
                    new ToDo { Task = "pack beach towels", Complete = true },
                    new ToDo { Task = "get snacks", Complete = false },
                    new ToDo { Task = "remember to bring gas rewards card", Complete = false },
                    new ToDo { Task = "purchase flights", Complete = false },
                },
                ImgURL = "assets/images/trips/anguila1.jpg"
            }
        };

    }

    public static List<Trip> GetAll() => Trips!; // should I be implementing IAsyncEnumerable<> and reimplement everything to be async?
    public static Trip? Get(int id) => Trips?.FirstOrDefault(t => t.Id == id);
    public static void Add(Trip trip)
    {
        // Note: A completely unique Trip Id will be generated from the frontend (already included within the Trip object)
        Trips!.Add(trip);
    }

    public static void Delete(int id)
    {
        var trip = Get(id);
        if (trip is null)
            return;

        Trips!.Remove(trip);
    }

    public static void Update(Trip trip)
    {
        var index = Trips!.FindIndex(t => t.Id == trip.Id);
        if (index == -1)
            return;

        Trips[index] = trip;
    }
}