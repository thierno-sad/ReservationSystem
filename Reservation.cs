namespace ReservationService.Models;

public class Reservation
{
    public int Id { get; set; }
    public string ClientName { get; set; } = "";
    public string ResourceName { get; set; } = "";
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Created";
}