namespace TravelTrack_API.Models;

public class Trip
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Details { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<Destination>? Destinations { get; set; }
    public IEnumerable<User>? Members { get; set; }
    public IEnumerable<ToDo>? ToDo { get; set; }
    public string? ImgURL { get; set; }
}