namespace PaymentService.Models;

public class PaymentRecord
{
    public int Id { get; set; }
    public string StripePaymentId { get; set; } = "";
    public long Amount { get; set; }
    public string Currency { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}