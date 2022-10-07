using TravelTrack_API.DTO;

namespace TravelTrack_API.Services;

/* 
 * NOTE: This file is being used in place of TripService.cs TEMPORARILY
 * This file is temporarily being used to deal with static data and allow my TripController to be fully functional for the time being.
 * The TripService.cs will be used w/ it's dependency injection for implementing automapper, context, Domain to DTO conversion
 * This file will be deleted in the near future.
*/
public static class TripServiceTEMP
{
    static List<Trip> Trips { get; }
    static TripServiceTEMP()
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
                    new Destination { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUser>
                {
                    new TripUser { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" },
                    new TripUser { Username = "dummyuser@dummy.dum", FirstName = "Dummy", LastName = "User" },
                    new TripUser { Username = "fakeyfake@fakey.fake", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDo>
                {
                    new ToDo { Id = 0, Task = "pack clothes", Complete = false },
                    new ToDo { Id = 1, Task = "get snacks", Complete = true },
                    new ToDo { Id = 2, Task = "finish booking resort", Complete = true },
                    new ToDo { Id = 3, Task = "buy more toothpaste", Complete = false },
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
                    new Destination { Id = "ChIJASFVO5VoAIkRGJbQtRWxD7w", City = "Myrtle Beach", Region = "South Carolina", Country = "United States" },
                    new Destination { Id = "ChIJdySo3EJ6_ogRa-zhruD3-jU", City = "Charleston", Region = "South Carolina", Country = "United States" },
                },
                Members = new List<TripUser>
                {
                    new TripUser { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" },
                    new TripUser { Username = "dummyuser@dummy.dum", FirstName = "Dummy", LastName = "User" },
                    new TripUser { Username = "fakeyfake@fakey.fake", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDo>
                {
                    new ToDo { Id = 0, Task = "buy new swim trunks", Complete = true },
                    new ToDo { Id = 1, Task = "pack beach towels", Complete = true },
                    new ToDo { Id = 2, Task = "get snacks", Complete = false },
                    new ToDo { Id = 3, Task = "remember to bring gas rewards card", Complete = false },
                    new ToDo { Id = 4, Task = "purchase flights", Complete = false },
                },
                ImgURL = "assets/images/trips/myrtlebeach1.jpg"
            },
            new Trip
            {
                Id = 3,
                Title = "Another Test Trip",
                Details = "",
                StartDate = new DateTime(2024, 7, 19),
                EndDate = new DateTime(2024, 8, 1),
                Destinations = new List<Destination>
                {
                    new Destination { Id = "ChIJASFVO5VoAIkRGJbQtRWxD7w", City = "Myrtle Beach", Region = "South Carolina", Country = "United States" },
                    new Destination { Id = "ChIJdySo3EJ6_ogRa-zhruD3-jU", City = "Charleston", Region = "South Carolina", Country = "United States" },
                },
                Members = new List<TripUser>
                {
                    new TripUser { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" },
                    new TripUser { Username = "dummyuser@dummy.dum", FirstName = "Dummy", LastName = "User" },
                    new TripUser { Username = "fakeyfake@fakey.fake", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDo>
                {
                    new ToDo { Id = 0, Task = "buy new swim trunks", Complete = true },
                    new ToDo { Id = 1, Task = "pack beach towels", Complete = true },
                    new ToDo { Id = 2, Task = "get snacks", Complete = false },
                },
                ImgURL = "assets/images/trips/myrtlebeach1.jpg"
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