using TravelTrack_API.DTO;

namespace TravelTrack_API.Services;

public class TripService: ITripService
{
    List<TripDto> Trips { get; }
    public TripService()
    {
        Trips = new List<TripDto>
        {
            new TripDto
            {
                Id = 1,
                Title = "Brothers' Anguila Trip",
                Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                StartDate = new DateTime(2022, 3, 15),
                EndDate = new DateTime(2022, 3, 20),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJw4OtEaZjDowRZCw_jCcczqI", City = "Zemi Beach", Region = "West End", Country = "Anguilla" }
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" },
                    new TripUserDto { Username = "dummyuser@dummy.dum", FirstName = "Dummy", LastName = "User" },
                    new TripUserDto { Username = "fakeyfake@fakey.fake", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Id = 0, Task = "pack clothes", Complete = false },
                    new ToDoDto { Id = 1, Task = "get snacks", Complete = true },
                    new ToDoDto { Id = 2, Task = "finish booking resort", Complete = true },
                    new ToDoDto { Id = 3, Task = "buy more toothpaste", Complete = false },
                },
                ImgURL = "assets/images/trips/anguila1.jpg"
            },
            new TripDto
            {
                Id = 2,
                Title = "Myrtle Beach and Charleston Family Vacay 2022",
                Details = "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.",
                StartDate = new DateTime(2022, 5, 27),
                EndDate = new DateTime(2022, 6, 5),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJASFVO5VoAIkRGJbQtRWxD7w", City = "Myrtle Beach", Region = "South Carolina", Country = "United States" },
                    new DestinationDto { Id = "ChIJdySo3EJ6_ogRa-zhruD3-jU", City = "Charleston", Region = "South Carolina", Country = "United States" },
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" },
                    new TripUserDto { Username = "dummyuser@dummy.dum", FirstName = "Dummy", LastName = "User" },
                    new TripUserDto { Username = "fakeyfake@fakey.fake", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Id = 0, Task = "buy new swim trunks", Complete = true },
                    new ToDoDto { Id = 1, Task = "pack beach towels", Complete = true },
                    new ToDoDto { Id = 2, Task = "get snacks", Complete = false },
                    new ToDoDto { Id = 3, Task = "remember to bring gas rewards card", Complete = false },
                    new ToDoDto { Id = 4, Task = "purchase flights", Complete = false },
                },
                ImgURL = "assets/images/trips/myrtlebeach1.jpg"
            },
            new TripDto
            {
                Id = 3,
                Title = "Another Test Trip",
                Details = "",
                StartDate = new DateTime(2024, 7, 19),
                EndDate = new DateTime(2024, 8, 1),
                Destinations = new List<DestinationDto>
                {
                    new DestinationDto { Id = "ChIJASFVO5VoAIkRGJbQtRWxD7w", City = "Myrtle Beach", Region = "South Carolina", Country = "United States" },
                    new DestinationDto { Id = "ChIJdySo3EJ6_ogRa-zhruD3-jU", City = "Charleston", Region = "South Carolina", Country = "United States" },
                },
                Members = new List<TripUserDto>
                {
                    new TripUserDto { Username = "jmoran@ceiamerica.com", FirstName = "Jonathan", LastName = "Moran" },
                    new TripUserDto { Username = "dummyuser@dummy.dum", FirstName = "Dummy", LastName = "User" },
                    new TripUserDto { Username = "fakeyfake@fakey.fake", FirstName = "Fake", LastName = "User" },
                },
                ToDo = new List<ToDoDto>
                {
                    new ToDoDto { Id = 0, Task = "buy new swim trunks", Complete = true },
                    new ToDoDto { Id = 1, Task = "pack beach towels", Complete = true },
                    new ToDoDto { Id = 2, Task = "get snacks", Complete = false },
                },
                ImgURL = "assets/images/trips/myrtlebeach1.jpg"
            }
        };
    }

    public List<TripDto> GetAll() => Trips!; 
    public TripDto Get(long id) => Trips?.FirstOrDefault(t => t.Id == id)!;
    public TripDto Add(TripDto trip)
    {
        Trips.Add(trip);
        return trip;
    }

    public void Delete(long id)
    {
        var trip = Get(id);
        if (trip is null)
            return;

        Trips!.Remove(trip);
    }

    public TripDto Update(TripDto trip)
    {
        var index = Trips!.FindIndex(t => t.Id == trip.Id);
        if (index == -1)
            return trip;

        Trips[index] = trip;
        return trip;
    }
}