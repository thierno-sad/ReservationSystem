namespace NotificationService.Models;

public class NotificationRecord
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Message { get; set; } = "";
    public string Status { get; set; } = "Sent";
    public DateTime SentAt { get; set; } = DateTime.Now;
}